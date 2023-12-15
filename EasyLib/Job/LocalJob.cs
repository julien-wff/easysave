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
/// <param name="type">Backup type (full, differential)</param>
/// <param name="state">Current state</param>
public class LocalJob(
    string name,
    string sourceFolder,
    string destinationFolder,
    JobType type,
    JobState state = JobState.End) : Job
{
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

    public override string DestinationFolder { get; set; } = destinationFolder;
    public override ulong FilesBytesCopied { get; set; }
    public override uint FilesCopied { get; set; }
    public override uint FilesCount { get; set; }
    public override ulong FilesSizeBytes { get; set; }
    public override uint Id { get; init; }
    public override string Name { get; set; } = name;
    public override string SourceFolder { get; set; } = sourceFolder;
    public override JobState State { get; set; } = state;
    public override JobType Type { get; set; } = type;
    public override string CurrentFileSource { get; set; } = string.Empty;
    public override string CurrentFileDestination { get; set; } = string.Empty;
    public override bool CurrentlyRunning { get; protected set; }
    public override CancellationTokenSource CancellationToken { get; protected set; } = new();

    public override void OnJobProgress(Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobProgress(job);
        }
    }

    /// <summary>
    /// Convert a job instance to a JsonJob object
    /// </summary>
    /// <returns>JsonJob object</returns>
    public override JsonJob ToJsonJob()
    {
        return new JsonJob
        {
            id = Id,
            name = Name,
            source_folder = SourceFolder,
            destination_folder = DestinationFolder,
            type = EnumConverter<JobType>.ConvertToString(Type),
            state = EnumConverter<JobState>.ConvertToString(State),
            active_job_info = State == JobState.End
                ? null
                : new JsonActiveJobInfo
                {
                    total_file_count = FilesCount,
                    total_file_size = FilesSizeBytes,
                    files_copied = FilesCopied,
                    bytes_copied = FilesBytesCopied,
                    current_file_source = CurrentFileSource,
                    current_file_destination = CurrentFileDestination
                }
        };
    }

    /// <summary>
    /// Start the backup job without cancelling it
    /// </summary>
    /// <returns>True if the job is started correctly</returns>
    public override bool Resume()
    {
        CancellationToken.Dispose();
        CancellationToken = new CancellationTokenSource();
        return Start(true);
    }

    /// <summary>
    /// Cancel the backup job and run
    /// </summary>
    /// <returns>True if the job is started correctly</returns>
    public override bool Run()
    {
        return Start(false);
    }

    /// <summary>
    /// Run the backup job
    /// </summary>
    /// <param name="resume">If false, the job is cancelled first</param>
    /// <returns>True when the job is complete</returns>
    protected override bool Start(bool resume)
    {
        // If the job is already running, return false
        if (CurrentlyRunning)
        {
            return false;
        }

        // Reset the job stats if the job is not resumed
        if (!resume)
        {
            _resetJobStats();
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
                JobSteps(transferManager, folders);
            }
            catch (Exception e)
            {
                CurrentlyRunning = false;
                Pause();
                _notifySubscribersForError(e);
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
    protected override void JobSteps(TransferManager transferManager, List<List<string>> folders)
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

    /// <summary>
    /// Pause a job execution
    /// </summary>
    /// <returns>True when the job is paused</returns>
    public override bool Pause()
    {
        CancellationToken.Cancel();
        return true;
    }

    /// <summary>
    /// Cancel a running or a paused job, make its status to "End"
    /// </summary>
    /// <returns></returns>
    public override bool Cancel()
    {
        CancellationToken.Cancel();
        _resetJobStats();
        return true;
    }

    protected override void _resetJobStats()
    {
        FilesCount = 0;
        FilesSizeBytes = 0;
        FilesCopied = 0;
        FilesBytesCopied = 0;
        State = JobState.End;
    }

    protected override void _notifySubscribersForChangeState(JobState subState)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(subState, this);
        }
    }

    protected override void _notifySubscribersForError(Exception error)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobError(error);
        }
    }

    protected override void _setJobState(JobState pubState)
    {
        State = pubState;
        _notifySubscribersForChangeState(pubState);
    }
}
