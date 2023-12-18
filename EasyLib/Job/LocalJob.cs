using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Json;

namespace EasyLib.Job;

/// <summary>
/// A job is a backup task that can be run by the application. It can be a copy, a move, a delete, etc.
/// </summary>
/// <param name="name">Name of the task</param>
/// <param name="sourceFolder">Folder to backup</param>
/// <param name="destinationFolder">Backup destination</param>
/// <param name="typeype">Backup type (full, differential)</param>
/// <param name="stateate">Current state</param>
public class LocalJob(
    string name,
    string sourceFolder,
    string destinationFolder,
    JobType type,
    JobState state = JobState.End) : Job(name, sourceFolder, destinationFolder, type, state)
{
    /// <summary>
    /// Lock for copying big files
    /// </summary>
    public static readonly Semaphore MaxSizeFileCopying = new(1, 1);

    /// <summary>
    /// Event for when jobs need to wait for the company software to close
    /// </summary>
    public static readonly EventWaitHandle NotifyWaitingJobs = new(initialState: false, EventResetMode.ManualReset);

    /// <summary>
    /// Number of current priority copies running (files with the listed extensions in the config file)
    /// </summary>
    public static ulong CurrentPriorityRunning;

    /// <summary>
    /// Create a job instance from a JsonJob object
    /// </summary>
    /// <param name="job">JsonJob object</param>
    public LocalJob(JsonJob job) : this(job.name, job.source_folder, job.destination_folder, JobType.Full)
    {
        Id = job.id;
        Type = EnumConverter<JobType>.ConvertToEnum(job.type);
        State = EnumConverter<JobState>.ConvertToEnum(job.state);
        FilesCount = job.active_job_info?.total_file_count ?? 0;
        FilesSizeBytes = job.active_job_info?.total_file_size ?? 0;
        FilesCopied = job.active_job_info?.files_copied ?? 0;
        FilesBytesCopied = job.active_job_info?.bytes_copied ?? 0;
        CurrentFileSource = job.active_job_info?.current_file_source ?? string.Empty;
        CurrentFileDestination = job.active_job_info?.current_file_destination ?? string.Empty;
    }

    /// <summary>
    /// Cancellation token to stop the job execution
    /// </summary>
    public CancellationTokenSource CancellationToken { get; private set; } = new();

    public override bool Resume()
    {
        CancellationToken.Dispose();
        CancellationToken = new CancellationTokenSource();
        return _executeJob();
    }

    public override bool Run()
    {
        _resetJobStats();
        return _executeJob();
    }

    /// <summary>
    /// Run the backup job
    /// </summary>
    /// <returns>True when the job is complete</returns>
    private bool _executeJob()
    {
        // If the job is already running, return false
        if (CurrentlyRunning)
        {
            return false;
        }

        // Create the transfer manager and the folder selector
        var transferManager = new TransferManager(this);
        var selector = BackupFolderSelectorFactory.Create(Type, State);
        var folderList = Directory.GetDirectories(DestinationFolder).ToList();
        var directories = new List<List<string>> { folderList };
        var lastFolder = "";
        if (folderList.Count != 0)
        {
            lastFolder = folderList[^1] + Path.DirectorySeparatorChar;
        }

        // Select the folders to backup
        var folders = selector.SelectFolders(directories, lastFolder, Type, DestinationFolder);

        // Run the job in a new thread
        var thread = new Thread(() =>
        {
            try
            {
                _runJobSteps(transferManager, folders);
            }
            catch (Exception e)
            {
                CurrentlyRunning = false;
                Pause();
                OnJobError(e);
            }
        });
        thread.Start();
        return true;
    }

    /// <summary>
    /// Run the different steps of the backup job
    /// </summary>
    /// <param name="transferManager">Instance of the transfer manager</param>
    /// <param name="folders">Folders used to compute the difference</param>
    private void _runJobSteps(TransferManager transferManager, List<List<string>> folders)
    {
        transferManager.Subscribe(this);
        CurrentlyRunning = true;

        if (!CancellationToken.IsCancellationRequested)
        {
            _setJobState(JobState.SourceScan);
            transferManager.ScanSource();
        }

        if (!CancellationToken.IsCancellationRequested)
        {
            _setJobState(JobState.DifferenceCalculation);
            transferManager.ComputeDifference(folders);
        }

        if (!CancellationToken.IsCancellationRequested)
        {
            _setJobState(JobState.DestinationStructureCreation);
            transferManager.CreateDestinationStructure();
        }

        if (!CancellationToken.IsCancellationRequested)
        {
            _setJobState(JobState.Copy);
            transferManager.TransferFiles();
        }

        CurrentlyRunning = false;

        // If the job is cancelled, re-send the state to the subscribers so they can update their UI
        // Otherwise, set the state to "End"
        _setJobState(!CancellationToken.IsCancellationRequested ? JobState.End : State);

        // Make the cancellation token available for the next job
        CancellationToken.Dispose();
        CancellationToken = new CancellationTokenSource();

        transferManager.Unsubscribe(this);
    }

    public override bool Pause()
    {
        CancellationToken.Cancel();
        return true;
    }

    public override bool Cancel()
    {
        CancellationToken.Cancel();
        _resetJobStats();
        return true;
    }

    /// <summary>
    /// Reset the characteristics of the job to their default values,
    /// like if it was just created
    /// </summary>
    private void _resetJobStats()
    {
        FilesCount = 0;
        FilesSizeBytes = 0;
        FilesCopied = 0;
        FilesBytesCopied = 0;
        State = JobState.End;
    }

    /// <summary>
    /// Change the state of the job and notify the subscribers
    /// </summary>
    /// <param name="pubState"></param>
    private void _setJobState(JobState pubState)
    {
        State = pubState;
        OnJobStateChange(pubState, this);
    }
}
