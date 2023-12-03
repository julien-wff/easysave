using EasyLib.Enums;
using EasyLib.Events;
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
    JobState state = JobState.End) : IJobStatusPublisher
{
    private readonly List<IJobStatusSubscriber> _observers = new();
    public string DestinationFolder = destinationFolder;
    public ulong FilesBytesCopied;
    public uint FilesCopied;
    public uint FilesCount;
    public ulong FilesSizeBytes;
    public uint Id;
    public string Name = name;
    public string SourceFolder = sourceFolder;
    public JobState State = state;
    public JobType Type = type;

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
    }

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        _observers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        _observers.Remove(subscriber);
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
                    bytes_copied = FilesBytesCopied
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
        return true;
    }
}
