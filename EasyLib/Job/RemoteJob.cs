using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Json;

namespace EasyLib.Job;

public class RemoteJob : Job
{
    public override string DestinationFolder { get; set; }
    public override ulong FilesBytesCopied { get; set; }
    public override uint FilesCopied { get; set; }
    public override uint FilesCount { get; set; }
    public override ulong FilesSizeBytes { get; set; }
    public override uint Id { get; init; }
    public override string Name { get; set; }
    public override string SourceFolder { get; set; }
    public override JobState State { get; set; }
    public override JobType Type { get; set; }
    public override string CurrentFileSource { get; set; }
    public override string CurrentFileDestination { get; set; }
    public override bool CurrentlyRunning { get; protected set; }
    public override CancellationTokenSource CancellationToken { get; protected set; }

    public override JsonJob ToJsonJob()
    {
        throw new NotImplementedException();
    }

    public override bool Resume()
    {
        throw new NotImplementedException();
    }

    public override bool Run()
    {
        throw new NotImplementedException();
    }

    protected override bool Start(bool resume)
    {
        throw new NotImplementedException();
    }

    protected override void JobSteps(TransferManager transferManager, List<List<string>> folders)
    {
        throw new NotImplementedException();
    }

    public override bool Pause()
    {
        throw new NotImplementedException();
    }

    public override bool Cancel()
    {
        throw new NotImplementedException();
    }

    public override void OnJobProgress(Job job)
    {
        throw new NotImplementedException();
    }

    protected override void _resetJobStats()
    {
        throw new NotImplementedException();
    }

    protected override void _notifySubscribersForChangeState(JobState subState)
    {
        throw new NotImplementedException();
    }

    protected override void _notifySubscribersForError(Exception error)
    {
        throw new NotImplementedException();
    }

    protected override void _setJobState(JobState pubState)
    {
        throw new NotImplementedException();
    }
}
