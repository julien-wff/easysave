using System.Text.RegularExpressions;
using EasyLib.Enums;
using EasyLib.Events;

namespace EasyLib.JobManager;

public abstract partial class JobManager : IJobStatusSubscriber, IJobStatusPublisher
{
    /// <summary>
    /// List of all available jobs
    /// </summary>
    protected readonly List<Job.Job> Jobs = [];


    /// <summary>
    /// Subscribers to the job-related events
    /// </summary>
    protected readonly List<IJobStatusSubscriber> Subscribers = [];


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
    public virtual void OnJobProgress(Job.Job job)
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
    public virtual void OnJobStateChange(JobState state, Job.Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(state, job);
        }
    }

    /// <summary>
    /// Regex to match a job index range, for example: 1-3,5
    /// </summary>
    [GeneratedRegex(@"^\d+-\d+$", RegexOptions.NonBacktracking)]
    private static partial Regex JobIndexRangeRegex();

    /// <summary>
    /// Parse a job string into a list of jobs. IDs and names are accepted.
    /// </summary>
    /// <param name="jobsIString">Job string, for instance: 1-3,5,job2</param>
    /// <returns>The list of found jobs</returns>
    public List<Job.Job> GetJobsFromString(string jobsIString)
    {
        var segments = jobsIString
            .Split(',')
            .Select(s => s.Trim());

        var jobs = new List<Job.Job>();

        foreach (var segment in segments)
        {
            if (int.TryParse(segment, out var id))
            {
                var job = Jobs.Find(job => job.Id == id);
                if (job != null && !jobs.Contains(job))
                {
                    jobs.Add(job);
                }
            }
            else if (JobIndexRangeRegex().IsMatch(segment))
            {
                var range = segment.Split('-');
                int firstNum = int.Parse(range[0]), lastNum = int.Parse(range[1]);
                var start = (uint)Math.Min(firstNum, lastNum);
                var end = (uint)Math.Max(firstNum, lastNum);
                var jobRange = Jobs.Where(job => job.Id >= start && job.Id <= end && !jobs.Contains(job)).ToList();
                jobs.AddRange(jobRange);
            }
            else
            {
                var job = Jobs.Find(job => job.Name == segment);
                if (job != null && !jobs.Contains(job))
                {
                    jobs.Add(job);
                }
            }
        }

        return jobs;
    }

    /// <summary>
    /// Get the jobs with the specified IDs
    /// </summary>
    /// <param name="jobIds">List of IDs to select</param>
    /// <returns>List of jobs</returns>
    public virtual List<Job.Job> GetJobsFromIds(IEnumerable<int> jobIds)
    {
        var list = jobIds.ToList();
        var ids = list.Select(id => (uint)id);
        return Jobs.Where(job => ids.Contains(job.Id)).ToList();
    }

    /// <summary>
    /// Get all the jobs
    /// </summary>
    /// <returns>Stored jobs</returns>
    public abstract List<Job.Job> GetJobs();

    /// <summary>
    /// Fetch the jobs from the source
    /// </summary>
    /// <returns>True if the operation is successful</returns>
    public abstract bool FetchJobs();

    /// <summary>
    /// Edit a job details
    /// </summary>
    /// <param name="job">Job to edit</param>
    /// <param name="name">New name</param>
    /// <param name="source">New source</param>
    /// <param name="destination">New Destination</param>
    /// <param name="type">New type</param>
    /// <returns>JobCheckRule.Valid if the change is effective, otherwise the error</returns>
    public abstract JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type);

    /// <summary>
    /// Check if a job can be created with the specified parameters
    /// </summary>
    /// <param name="id">ID of the job (-1 for a new job)</param>
    /// <param name="name">Name</param>
    /// <param name="source">Source folder</param>
    /// <param name="destination">Destination folder</param>
    /// <param name="testEmpty">Check if the destination folder is empty</param>
    /// <returns>JobCheckRule.Valid if the parameters are valid, otherwise the error</returns>
    public abstract JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true);

    /// <summary>
    /// Start the execution of the specified jobs. If the jobs are paused, they reset and re-run.
    /// </summary>
    /// <param name="jobs">List of jobs to start</param>
    /// <returns></returns>
    public virtual bool ExecuteJobs(IEnumerable<Job.Job> jobs)
    {
        return jobs
            .Select(job => job.Run())
            .Aggregate(true, (current, result) => current && result);
    }

    public virtual void ExecuteJob(Job.Job job)
    {
        job.Run();
    }

    /// <summary>
    /// Create a new job
    /// </summary>
    /// <param name="name">Job name</param>
    /// <param name="src">Backup source UNC path</param>
    /// <param name="dest">Backup destination UNC path</param>
    /// <param name="type">Backup type</param>
    /// <returns>Newly created job</returns>
    public abstract Job.Job CreateJob(string name, string src, string dest, JobType type);

    /// <summary>
    /// Delete a job
    /// </summary>
    /// <param name="job">Job to delete</param>
    public abstract void DeleteJob(Job.Job job);

    /// <summary>
    /// Cancel a paused job, make its status to "End"
    /// </summary>
    /// <param name="job">Job to cancel</param>
    public abstract void CancelJob(Job.Job job);

    /// <summary>
    /// Pause a running job
    /// </summary>
    /// <param name="job">Job to cancel</param>
    public abstract void PauseJob(Job.Job job);

    /// <summary>
    /// Resume a paused job
    /// </summary>
    /// <param name="job">Job to pause</param>
    public abstract void ResumeJob(Job.Job job);

    /// <summary>
    /// Pause all running jobs
    /// </summary>
    public abstract void PauseAllJobs();

    /// <summary>
    /// Resume all paused jobs
    /// </summary>
    public abstract void ResumeAllJobs();

    /// <summary>
    /// Restarts the job manager with the new configuration
    /// </summary>
    public virtual void ReloadConfig()
    {
        // Implementation optional
    }

    /// <summary>
    /// Clean stop of the job manager
    /// </summary>
    public virtual void CleanStop()
    {
        // Implementation optional
    }
}
