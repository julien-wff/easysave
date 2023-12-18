using EasyLib.Enums;

namespace EasyLib.Job;

public class RemoteJob(string name, string source, string destination, JobType type)
    : Job(name, source, destination, type)
{
    public override bool Resume()
    {
        throw new NotImplementedException();
    }

    public override bool Run()
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
}
