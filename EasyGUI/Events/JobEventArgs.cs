using EasyLib.Job;

namespace EasyGUI.Events;

public class JobEventArgs(Job job) : EventArgs
{
    public Job Job { get; } = job;
}
