using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Files;
using EasyLib.Json;

namespace EasyLib.Job;

public abstract class Job : IJobStatusPublisher, IJobStatusSubscriber
{
    public static readonly Semaphore MaxSizeFileCopying = new Semaphore(1, 1);

    public static readonly EventWaitHandle NotifyWaitingJobs =
        new EventWaitHandle(initialState: false, EventResetMode.ManualReset);

    public static ulong CurrentPriorityRunning;
    public abstract string DestinationFolder { get; set; }
    public abstract ulong FilesBytesCopied { get; set; }
    public abstract uint FilesCopied { get; set; }
    public abstract uint FilesCount { get; set; }
    public abstract ulong FilesSizeBytes { get; set; }
    public abstract uint Id { get; init; }
    public abstract string Name { get; set; }
    public abstract string SourceFolder { get; set; }
    public abstract JobState State { get; set; }
    public abstract JobType Type { get; set; }
    public abstract string CurrentFileSource { get; set; }
    public abstract string CurrentFileDestination { get; set; }
    public abstract bool CurrentlyRunning { get; protected set; }
    public abstract CancellationTokenSource CancellationToken { get; protected set; }
    protected List<IJobStatusSubscriber> Subscribers { get; } = new();

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        Subscribers.Remove(subscriber);
    }

    public abstract void OnJobProgress(Job job);

    public abstract JsonJob ToJsonJob();
    public abstract bool Resume();
    public abstract bool Run();
    protected abstract bool Start(bool resume);
    protected abstract void JobSteps(TransferManager transferManager, List<List<string>> folders);
    public abstract bool Pause();
    public abstract bool Cancel();
    protected abstract void _resetJobStats();
    protected abstract void _notifySubscribersForChangeState(JobState subState);
    protected abstract void _notifySubscribersForError(Exception error);
    protected abstract void _setJobState(JobState pubState);
}
