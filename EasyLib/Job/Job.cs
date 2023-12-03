using EasyLib.Enums;
using EasyLib.Events;

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
    public ulong FilesBytesCopied = 0;
    public uint FilesCopied = 0;
    public uint FilesCount = 0;
    public ulong FilesSizeBytes = 0;
    public uint Id;
    public string Name = name;
    public string SourceFolder = sourceFolder;
    public JobState State = state;
    public JobType Type = type;

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        _observers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        _observers.Remove(subscriber);
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
