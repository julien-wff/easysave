using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Files;

namespace EasyLib;

/// <summary>
/// Facade to all the job-related work
/// </summary>
public class JobManager : IJobStatusSubscriber, IJobStatusPublisher
{
    /// <summary>
    /// List of all available jobs
    /// </summary>
    private readonly List<Job.Job> _jobs = new();

    /// <summary>
    /// List of all the subscribers to the job-related events
    /// </summary>
    private readonly List<IJobStatusSubscriber> _subscribers = new();

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Get all the jobs
    /// </summary>
    /// <returns>Stored jobs</returns>
    public List<Job.Job> GetJobs()
    {
        return _jobs;
    }

    /// <summary>
    /// Populate the list of jobs from the state.json file
    /// </summary>
    /// <returns>True if the operation is successful</returns>
    public bool FetchJobs()
    {
        _jobs.Clear();
        _jobs.AddRange(StateManager.Instance.ReadJobs());
        return true;
    }

    /// <summary>
    /// Parse a job string into a list of jobs. IDs and names are accepted.
    /// </summary>
    /// <param name="jobsIString">Job string, for instance: 1-3,5,job2</param>
    /// <returns>The list of found jobs</returns>
    public List<Job.Job> GetJobsFromString(string jobsIString)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the jobs with the specified IDs
    /// </summary>
    /// <param name="jobIds">List of job IDs</param>
    /// <returns></returns>
    public List<Job.Job> GetJobsFromIds(IEnumerable<int> jobIds)
    {
        var ids = jobIds.ToList().Select(id => (uint)id);
        return _jobs.Where(job => ids.Contains(job.Id)).ToList();
    }

    /// <summary>
    /// Start the execution of the specified jobs. If the jobs are paused, they are resumed.
    /// </summary>
    /// <param name="jobIds">IDs of the jobs to start</param>
    /// <returns>True if the execution is complete</returns>
    public bool ExecuteJobs(IEnumerable<int> jobIds)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Start the execution of the specified jobs. If the jobs are paused, they are resumed.
    /// </summary>
    /// <param name="jobs">List of obs to start</param>
    /// <returns></returns>
    public bool ExecuteJobs(IEnumerable<Job.Job> jobs)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Create a new job
    /// </summary>
    /// <param name="name">Job name</param>
    /// <param name="src">Backup source UNC path</param>
    /// <param name="dest">Backup destination UNC path</param>
    /// <param name="type">Backup type</param>
    /// <returns>Newly created job</returns>
    public Job.Job CreateJob(string name, string src, string dest, JobType type)
    {
        var highestId = _jobs.Count > 0 ? _jobs.Max(job => job.Id) : 0;
        var newJob = new Job.Job(name, src, dest, type)
        {
            Id = highestId + 1
        };
        _jobs.Add(newJob);
        StateManager.Instance.WriteJobs(_jobs);
        return newJob;
    }

    /// <summary>
    /// Delete a job
    /// </summary>
    /// <param name="job">Job to delete</param>
    public void DeleteJob(Job.Job job)
    {
        _jobs.Remove(job);
    }

    /// <summary>
    /// Cancel a paused job, make its status to "End"
    /// </summary>
    /// <param name="job">Job to cancel</param>
    public void CancelJob(Job.Job job)
    {
        job.Cancel();
    }

    public JobCheckRules CheckJobRules(int id, string name, string source, string destination, bool testEmpty = true)
    {
        if (!Path.IsPathFullyQualified(source))
        {
            return JobCheckRules.SourcePathInvalid;
        }

        if (!Path.IsPathFullyQualified(destination))
        {
            return JobCheckRules.DestinationPathInvalid;
        }

        if (!Path.Exists(source))
        {
            return JobCheckRules.SourcePathDoesNotExist;
        }

        if (!Path.Exists(destination))
        {
            return JobCheckRules.DestinationPathDoesNotExist;
        }

        if (source.StartsWith(destination) || destination.StartsWith(source))
        {
            return JobCheckRules.SharedRoot;
        }

        var idCount = _jobs.Count(job => job.Id == id);

        if (idCount > 1)
        {
            return JobCheckRules.DuplicateId;
        }

        var jobs = _jobs.Where(job => job.Id != id).ToList();

        if (jobs.Exists(job => job.Name == name))
        {
            return JobCheckRules.DuplicateName;
        }

        if (jobs.Exists(job => job.SourceFolder == source && job.DestinationFolder == destination))
        {
            return JobCheckRules.DuplicatePaths;
        }

        if (testEmpty && Directory.EnumerateFileSystemEntries(destination).Any())
        {
            return JobCheckRules.DestinationNotEmpty;
        }

        return JobCheckRules.Valid;
    }
}
