namespace EasyLib.Events;

public interface IJobStatusPublisher
{
    /// <summary>
    /// Subscribe to the job-related events
    /// </summary>
    /// <param name="subscriber">Class that will be notified of the events</param>
    public void Subscribe(IJobStatusSubscriber subscriber);

    /// <summary>
    /// Unsubscribe from the job-related events
    /// </summary>
    /// <param name="subscriber">Class that will stop to be notified</param>
    public void Unsubscribe(IJobStatusSubscriber subscriber);
}
