using EasyLib.Enums;

namespace EasyLib;

public class RemoteJobManager : JobManager
{
    public override void OnJobStateChange(JobState state, Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override List<Job.Job> GetJobs()
    {
        throw new NotImplementedException();
    }

    public override bool FetchJobs()
    {
        throw new NotImplementedException();
    }

    public override List<Job.Job> GetJobsFromString(string jobsIString)
    {
        throw new NotImplementedException();
    }

    public override List<Job.Job> GetJobsFromIds(IEnumerable<int> jobIds)
    {
        throw new NotImplementedException();
    }

    public override JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type)
    {
        throw new NotImplementedException();
    }

    public override bool ExecuteJobs(IEnumerable<int> jobIds)
    {
        throw new NotImplementedException();
    }

    public override bool ExecuteJobs(IEnumerable<Job.Job> jobs)
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

    public override void PauseAllJobs()
    {
        throw new NotImplementedException();
    }

    public override JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true)
    {
        throw new NotImplementedException();
    }

    public override void ResumeJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void ResumeAllJobs()
    {
        throw new NotImplementedException();
    }

    public override void ReloadConfig()
    {
        throw new NotImplementedException();
    }

    public override void StopCheck()
    {
        throw new NotImplementedException();
    }

    public override void OnJobError(Exception error)
    {
        throw new NotImplementedException();
    }

    public override void OnJobProgress(Job.Job job)
    {
        throw new NotImplementedException();
    }
}
