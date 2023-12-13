using System.Diagnostics;
using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Files;
using EasyLib.Json;
using static System.Threading.EventResetMode;

namespace EasyLib.Job;

/// <summary>
/// A job is a backup task that can be run by the application. It can be a copy, a move, a delete, etc.
/// </summary>
/// <param name="name">Name of the task</param>
/// <param name="sourceFolder">Folder to backup</param>
/// <param name="destinationFolder">Backup destination</param>
/// <param name="type">Backup type (full, differential)</param>
/// <param name="state">Current state</param>
public class Job(
    string name,
    string sourceFolder,
    string destinationFolder,
    JobType type,
    JobState state = JobState.End) : IJobStatusPublisher, IJobStatusSubscriber
{
    public static readonly Semaphore MaxSizeFileCopying = new Semaphore(1, 1);
    public static uint CurrentPriorityRunning = 0;
    public static readonly EventWaitHandle NotifyWaitingJobs = new EventWaitHandle(initialState: false, ManualReset);

    /// <summary>
    /// Create a job instance from a JsonJob object
    /// </summary>
    /// <param name="job">JsonJob object</param>
    public Job(JsonJob job) : this(job.name, job.source_folder, job.destination_folder, JobType.Full)
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

    private List<IJobStatusSubscriber> Subscribers { get; } = new();
    public string DestinationFolder { get; set; } = destinationFolder;
    public ulong FilesBytesCopied { get; set; }
    public uint FilesCopied { get; set; }
    public uint FilesCount { get; set; }
    public ulong FilesSizeBytes { get; set; }
    public uint Id { get; init; }
    public string Name { get; set; } = name;
    public string SourceFolder { get; set; } = sourceFolder;
    public JobState State { get; set; } = state;
    public JobType Type { get; set; } = type;
    public string CurrentFileSource { get; set; } = string.Empty;
    public string CurrentFileDestination { get; set; } = string.Empty;
    public bool CurrentlyRunning { get; private set; }

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Remove(subscriber);
    }

    public void OnJobProgress(Job job)
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
    public JsonJob ToJsonJob()
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
    public bool Resume()
    {
        return Start(true);
    }

    /// <summary>
    /// Cancel the backup job and run
    /// </summary>
    /// <returns>True if the job is started correctly</returns>
    public bool Run()
    {
        return Start(false);
    }

    /// <summary>
    /// Run the backup job
    /// </summary>
    /// <param name="resume">If false, the job is cancelled first</param>
    /// <returns>True when the job is complete</returns>
    private bool Start(bool resume)
    {
        // If the job is already running, return false
        if (CurrentlyRunning)
        {
            return false;
        }

        // Wait for the company software to be closed
        if (ConfigManager.Instance.CheckProcessRunning())
        {
            var processName = Path.GetFileNameWithoutExtension(ConfigManager.Instance.CompanySoftwareProcessPath);
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                process.WaitForExit();
            }
        }

        // Reset the job stats if the job is not resumed
        if (!resume)
        {
            Cancel();
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
        var thread = new Thread(() => JobSteps(transferManager, folders));
        thread.Start();
        return true;
    }

    /// <summary>
    /// Run the different steps of the backup job
    /// </summary>
    /// <param name="transferManager">Instance of the transfer manager</param>
    /// <param name="folders">Folders used to compute the difference</param>
    private void JobSteps(TransferManager transferManager, List<List<string>> folders)
    {
        transferManager.Subscribe(this);
        CurrentlyRunning = true;
        _setJobState(JobState.SourceScan);
        transferManager.ScanSource();
        _setJobState(JobState.DifferenceCalculation);
        transferManager.ComputeDifference(folders);
        _setJobState(JobState.DestinationStructureCreation);
        transferManager.CreateDestinationStructure();
        _setJobState(JobState.Copy);
        transferManager.TransferFiles();
        CurrentlyRunning = false;
        _setJobState(JobState.End);
        transferManager.Unsubscribe(this);
    }

    /// <summary>
    /// Pause a job execution
    /// </summary>
    /// <returns>True when the job is paused</returns>
    public bool Pause()
    {
        return true;
    }

    /// <summary>
    /// Cancel a running or a paused job, make its status to "End"
    /// </summary>
    /// <returns></returns>
    public bool Cancel()
    {
        FilesCount = 0;
        FilesSizeBytes = 0;
        FilesCopied = 0;
        FilesBytesCopied = 0;
        State = JobState.End;
        return true;
    }

    private void _notifySubscribersForChangeState(JobState subState)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(subState, this);
        }
    }

    private void _setJobState(JobState pubState)
    {
        State = pubState;
        _notifySubscribersForChangeState(pubState);
    }
}
