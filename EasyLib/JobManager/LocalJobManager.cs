using EasyLib.Api;
using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Files;

namespace EasyLib.JobManager;

/// <summary>
/// Facade to all the job-related work
/// </summary>
public sealed class LocalJobManager : JobManager
{
    /// <summary>
    /// Don't save the changes to the state.json file. Everything is lost on restart.
    /// For test purposes only
    /// </summary>
    private readonly bool _ramOnly;

    private readonly JobManagerServer? _server;

    /// <summary>
    /// Event for when jobs need to wait for the company software to close
    /// </summary>
    private ProcessStartEvent _pauseJobEvent;

    public LocalJobManager(bool ramOnly = false)
    {
        _ramOnly = ramOnly;
        if (!ramOnly)
        {
            FetchJobs();
        }

        _pauseJobEvent = new ProcessStartEvent(ConfigManager.Instance.CompanySoftwareProcessPath, this);
        if (ConfigManager.Instance.ServerPort != null)
        {
            _server = new JobManagerServer(this);
        }
    }

    public override void OnJobProgress(Job.Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobProgress(job);
        }

        if (job.State == JobState.Copy)
        {
            StateManager.Instance.WriteJobs(Jobs);
        }

        _server?.Broadcast(ApiAction.Progress, job.ToJsonJob());
    }

    public override void OnJobStateChange(JobState state, Job.Job job)
    {
        foreach (var subscriber in Subscribers)
        {
            subscriber.OnJobStateChange(state, job);
        }

        StateManager.Instance.WriteJobs(Jobs);

        _server?.Broadcast(ApiAction.State, job.ToJsonJob());
    }

    public override void CleanStop()
    {
        _pauseJobEvent.Stop();
        _server?.CancellationTokenSource.Cancel();
        _server?.CleanInstance();
    }

    public override List<Job.Job> GetJobs()
    {
        return Jobs;
    }

    /// <summary>
    /// Populate the list of jobs from the state.json file
    /// </summary>
    /// <returns>True if the operation is successful</returns>
    public override bool FetchJobs()
    {
        Jobs.Clear();
        Jobs.AddRange(StateManager.Instance.ReadJobs());

        foreach (var job in Jobs)
        {
            job.Subscribe(this);
        }

        return true;
    }

    /// <summary>
    /// Edit a job details
    /// </summary>
    /// <param name="job">Job to edit</param>
    /// <param name="name">New name</param>
    /// <param name="source">New source folder</param>
    /// <param name="destination">New destination folder</param>
    /// <param name="type">New backup type</param>
    /// <returns></returns>
    public override JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type)
    {
        var check = CheckJobRules((int)job.Id, name, source, destination, false);
        if (check != JobCheckRule.Valid)
        {
            return check;
        }

        job.Cancel();

        job.Name = name;
        job.SourceFolder = source;
        job.DestinationFolder = destination;

        if (type != null)
        {
            job.Type = type.Value;
        }

        StateManager.Instance.WriteJobs(Jobs);
        return JobCheckRule.Valid;
    }

    public override Job.Job CreateJob(string name, string src, string dest, JobType type)
    {
        var highestId = Jobs.Count > 0 ? Jobs.Max(job => job.Id) : 0;
        var newJob = new Job.LocalJob(name, src, dest, type)
        {
            Id = highestId + 1
        };
        Jobs.Add(newJob);

        if (!_ramOnly)
        {
            StateManager.Instance.WriteJobs(Jobs);
        }

        newJob.Subscribe(this);

        return newJob;
    }

    public override void DeleteJob(Job.Job job)
    {
        job.Unsubscribe(this);
        Jobs.Remove(job);
        if (!_ramOnly)
        {
            StateManager.Instance.WriteJobs(Jobs);
        }
    }

    public override void CancelJob(Job.Job job)
    {
        job.Cancel();
        StateManager.Instance.WriteJobs(Jobs);
    }

    public override void PauseJob(Job.Job job)
    {
        job.Pause();
        StateManager.Instance.WriteJobs(Jobs);
    }

    public override void PauseAllJobs()
    {
        foreach (var job in Jobs.Where(job => job.State != JobState.End && job.CurrentlyRunning))
        {
            PauseJob(job);
        }
    }

    public override JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true)
    {
        if (!Path.IsPathFullyQualified(source))
        {
            return JobCheckRule.SourcePathInvalid;
        }

        if (!Path.IsPathFullyQualified(destination))
        {
            return JobCheckRule.DestinationPathInvalid;
        }

        if (!Path.Exists(source))
        {
            return JobCheckRule.SourcePathDoesNotExist;
        }

        if (!Path.Exists(destination))
        {
            return JobCheckRule.DestinationPathDoesNotExist;
        }

        if (source.StartsWith(destination + Path.DirectorySeparatorChar) ||
            destination.StartsWith(source + Path.DirectorySeparatorChar))
        {
            return JobCheckRule.SharedRoot;
        }

        var idCount = Jobs.Count(job => job.Id == id);

        if (idCount > 1)
        {
            return JobCheckRule.DuplicateId;
        }

        var jobs = Jobs.Where(job => job.Id != id).ToList();

        if (jobs.Exists(job => job.Name == name))
        {
            return JobCheckRule.DuplicateName;
        }

        if (jobs.Exists(job => job.SourceFolder == source && job.DestinationFolder == destination))
        {
            return JobCheckRule.DuplicatePaths;
        }

        if (testEmpty && Directory.EnumerateFileSystemEntries(destination).Any())
        {
            return JobCheckRule.DestinationNotEmpty;
        }

        return JobCheckRule.Valid;
    }

    public override void ResumeJob(Job.Job job)
    {
        job.Resume();
    }

    public override void ResumeAllJobs()
    {
        foreach (var job in Jobs.Where(job => job.State != JobState.End))
        {
            job.Resume();
        }
    }

    public override void ReloadConfig()
    {
        _pauseJobEvent.Stop();
        _pauseJobEvent = new ProcessStartEvent(ConfigManager.Instance.CompanySoftwareProcessPath, this);
    }
}
