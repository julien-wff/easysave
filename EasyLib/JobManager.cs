using EasyLib.Enums;
using EasyLib.Events;

namespace EasyLib;

public abstract class JobManager : IJobStatusSubscriber, IJobStatusPublisher
{
    /// <summary>
    /// List of all available jobs
    /// </summary>
    protected readonly List<Job.Job> _jobs = new();

    /// <summary>
    /// List of all the subscribers to the job-related events
    /// </summary>
    protected readonly List<IJobStatusSubscriber> _subscribers = new();


    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public abstract void OnJobError(Exception error);
    public abstract void OnJobProgress(Job.Job job);
    public abstract void OnJobStateChange(JobState state, Job.Job job);
    public abstract List<Job.Job> GetJobs();
    public abstract bool FetchJobs();
    public abstract List<Job.Job> GetJobsFromString(string jobsIString);
    public abstract List<Job.Job> GetJobsFromIds(IEnumerable<int> jobIds);
    public abstract JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type);
    public abstract bool ExecuteJobs(IEnumerable<int> jobIds);
    public abstract bool ExecuteJobs(IEnumerable<Job.Job> jobs);
    public abstract Job.Job CreateJob(string name, string src, string dest, JobType type);
    public abstract void DeleteJob(Job.Job job);
    public abstract void CancelJob(Job.Job job);
    public abstract void PauseJob(Job.Job job);
    public abstract void PauseAllJobs();

    public abstract JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true);

    public abstract void ResumeJob(Job.Job job);
    public abstract void ResumeAllJobs();
    public abstract void ReloadConfig();
    public abstract void StopCheck();
}
