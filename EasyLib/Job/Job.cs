using EasyLib.Enums;
using EasyLib.Events;
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
public class Job(string name, string sourceFolder, string destinationFolder, JobType type,
    JobState state = JobState.End) : IJobStatusPublisher, IJobStatusSubscriber
{
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
    public string DestinationFolder { get; } = destinationFolder;
    public ulong FilesBytesCopied { get; set; }
    public uint FilesCopied { get; set; }
    public uint FilesCount { get; set; }
    public ulong FilesSizeBytes { get; set; }
    public uint Id { get; init; }
    public string Name { get; } = name;
    public string SourceFolder { get; } = sourceFolder;
    public JobState State { get; set; } = state;
    public JobType Type { get; } = type;
    public string CurrentFileSource { get; set; } = string.Empty;
    public string CurrentFileDestination { get; set; } = string.Empty;

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
    /// Check if the job is valid (name, source and destination reachable, etc.)
    /// </summary>
    /// <returns>True if the job is valid</returns>
    public bool Check()
    {
        return true;
    }

    /// <summary>
    /// Run the backup job
    /// </summary>
    /// <returns>True when the job is complete</returns>
    public bool Run()
    {
        var tm = new TransferManager(this);
        var selector = BackupFolderSelectorFactory.Create(Type, State);
        var folderList = Directory.GetDirectories(DestinationFolder).ToList();
        var directories = new List<List<string>>() { folderList };
        var lastFolder = "";
        if (folderList.Any())
        {
            lastFolder = folderList[^1] + Path.DirectorySeparatorChar;
        }

        var folders = selector.SelectFolders(directories, lastFolder, Type, DestinationFolder);
        tm.Subscribe(this);
        _setJobState(JobState.SourceScan);
        tm.ScanSource();
        _setJobState(JobState.DifferenceCalculation);
        tm.ComputeDifference(folders);
        _setJobState(JobState.DestinationStructureCreation);
        tm.CreateDestinationStructure();
        _setJobState(JobState.Copy);
        tm.TransferFiles();
        _setJobState(JobState.End);
        tm.Unsubscribe(this);

        return true;
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

    private void _notifySubscribersForChangeState(JobState state)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(state, this);
        }
    }

    private void _setJobState(JobState state)
    {
        State = state;
        _notifySubscribersForChangeState(state);
    }
}
