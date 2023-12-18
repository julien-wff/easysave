using EasyLib.Enums;

namespace EasyLib.JobManager;

public class RemoteJobManager : JobManager
{
    public override List<Job.Job> GetJobs()
    {
        throw new NotImplementedException();
    }

    public override bool FetchJobs()
    {
        throw new NotImplementedException();
    }

    public override JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type)
    {
        throw new NotImplementedException();
    }

    public override JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true)
    {
        throw new NotImplementedException();
    }

    public override Job.Job CreateJob(string name, string src, string dest, JobType type)
    {
        throw new NotImplementedException();
    }

    public override void DeleteJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void CancelJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void PauseJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void ResumeJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void PauseAllJobs()
    {
        throw new NotImplementedException();
    }

    public override void ResumeAllJobs()
    {
        throw new NotImplementedException();
    }
}
