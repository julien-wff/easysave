using System.Management;

namespace EasyLib.Events;

public class ProcessStartEvent
{
    private readonly JobManager _jobManager;
    private readonly ManagementEventWatcher _processStartEvent;
    private readonly ManagementEventWatcher _processStopEvent;
    private int _numberOfRunningProcesses;

    /// <summary>
    /// create the start and stop event for a process and bind the event to the job manager
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="jobManager"></param>
    public ProcessStartEvent(string? processName, JobManager jobManager)
    {
        // check if system is running Windows
        _jobManager = jobManager;
#pragma warning disable CA1416
        _processStartEvent =
            new ManagementEventWatcher(
                new WqlEventQuery($"SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName = '{processName}'"));
        _processStartEvent.EventArrived += ProcessStarted;
        // on stop
        _processStopEvent = new ManagementEventWatcher(new WqlEventQuery(
            $"SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName = '{processName}'"));
        _processStopEvent.EventArrived += ProcessStopped;
        _processStartEvent.Start();
        _processStopEvent.Start();
#pragma warning restore CA1416
    }

    private void ProcessStarted(object sender, EventArrivedEventArgs e)
    {
        if (_numberOfRunningProcesses == 0)
        {
            _jobManager.PauseAllJobs();
            Interlocked.Increment(ref _numberOfRunningProcesses);
        }
        else
        {
            Interlocked.Increment(ref _numberOfRunningProcesses);
        }
    }

    private void ProcessStopped(object sender, EventArrivedEventArgs e)
    {
        if (_numberOfRunningProcesses == 1)
        {
            _jobManager.ResumeAllJobs();
            Interlocked.Decrement(ref _numberOfRunningProcesses);
        }
        else
        {
            Interlocked.Decrement(ref _numberOfRunningProcesses);
        }
    }

    public void Stop()
    {
#pragma warning disable CA1416
        _processStopEvent.Stop();
        _processStartEvent.Stop();
#pragma warning restore CA1416
    }
}
