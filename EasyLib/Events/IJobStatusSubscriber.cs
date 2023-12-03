using EasyLib.Enums;

namespace EasyLib.Events;

public interface IJobStatusSubscriber
{
    /// <summary>
    /// Event triggered when the job state changes (running, paused, steps...)
    /// </summary>
    /// <param name="state">New step of the job</param>
    /// <param name="job">Concerned job</param>
    public void OnJobStateChange(JobState state, Job.Job job)
    {
        // Implementation optional
    }

    /// <summary>
    /// Event triggered when an error occurs during the job
    /// </summary>
    /// <param name="error"></param>
    public void OnJobError(string error)
    {
        // Implementation optional
    }

    /// <summary>
    /// Event triggered when the job progress changes (number of files, bytes, etc.)
    /// </summary>
    /// <param name="job">Concerned job</param>
    public void OnJobProgress(Job.Job job)
    {
        // Implementation optional
    }
}
