using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Json;

namespace EasyLib.Job;

public abstract class Job(
    string name,
    string sourceFolder,
    string destinationFolder,
    JobType type,
    JobState state = JobState.End) : IJobStatusPublisher, IJobStatusSubscriber
{
    /// <summary>
    /// Destination folder of the job
    /// </summary>
    public string DestinationFolder { get; set; } = destinationFolder;

    /// <summary>
    /// Number of bytes currently copied during the execution of the job
    /// </summary>
    public ulong FilesBytesCopied { get; set; }

    /// <summary>
    /// Number of files currently copied during the execution of the job
    /// </summary>
    public uint FilesCopied { get; set; }

    /// <summary>
    /// Total number of files to copy
    /// </summary>
    public uint FilesCount { get; set; }

    /// <summary>
    /// Total size of the files to copy
    /// </summary>
    public ulong FilesSizeBytes { get; set; }

    /// <summary>
    /// ID of the job
    /// </summary>
    public uint Id { get; init; }

    /// <summary>
    /// Name of the job
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Source folder of the job
    /// </summary>
    public string SourceFolder { get; set; } = sourceFolder;

    /// <summary>
    /// Current state of the job
    /// </summary>
    public JobState State { get; set; } = state;

    /// <summary>
    /// Backup type (full, incremental, differential)
    /// </summary>
    public JobType Type { get; set; } = type;

    /// <summary>
    /// Source of current file being copied
    /// </summary>
    public string? CurrentFileSource { get; set; }

    /// <summary>
    /// Destination of current file being copied
    /// </summary>
    public string? CurrentFileDestination { get; set; }

    /// <summary>
    /// True if the job is currently running, false otherwise
    /// If it's false but the state is not End, it means that the job is paused
    /// </summary>
    public bool CurrentlyRunning { get; set; }

    /// <summary>
    /// Subscribers to the job-related events
    /// </summary>
    private List<IJobStatusSubscriber> Subscribers { get; } = [];

    /// <summary>
    /// Make the class subscribe to the job-related events
    /// </summary>
    /// <param name="subscriber">Instance to subscribe</param>
    public virtual void Subscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Add(subscriber);
    }

    /// <summary>
    /// Make the class unsubscribe from the job-related events
    /// </summary>
    /// <param name="subscriber">Instance to unsubscribe</param>
    public virtual void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Notify the subscribers that the job progression has changed
    /// (generally when a file has been copied)
    /// </summary>
    /// <param name="job">Instance of the running job</param>
    public virtual void OnJobProgress(Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobProgress(job);
        }
    }

    /// <summary>
    /// Notify the subscribers that an error occurred during the execution of the job
    /// </summary>
    /// <param name="error"></param>
    public virtual void OnJobError(Exception error)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobError(error);
        }
    }

    /// <summary>
    /// Notify the subscribers that the state of the job has changed
    /// </summary>
    /// <param name="state"></param>
    /// <param name="job"></param>
    public virtual void OnJobStateChange(JobState state, Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(state, job);
        }
    }

    /// <summary>
    /// Convert the job to a JsonJob for serialization
    /// If the job is currently running, the active_job_info field is filled
    /// </summary>
    /// <returns>JsonJob instance</returns>
    public virtual JsonJob ToJsonJob()
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
                    current_file_source = CurrentFileSource!,
                    current_file_destination = CurrentFileDestination!
                }
        };
    }

    /// <summary>
    /// Resume the backup job without cancelling it
    /// </summary>
    /// <returns>True if the job is resumed correctly</returns>
    public abstract bool Resume();

    /// <summary>
    /// Start the backup. If the job is already running, it is cancelled and restarted
    /// </summary>
    /// <returns>True if the job is started correctly</returns>
    public abstract bool Run();

    /// <summary>
    /// Pause a job execution
    /// </summary>
    /// <returns>True if the operation is successful</returns>
    public abstract bool Pause();

    /// <summary>
    /// Cancel a running or a paused job, make its status to "End"
    /// </summary>
    /// <returns>True if the operation is successful</returns>
    public abstract bool Cancel();
}
