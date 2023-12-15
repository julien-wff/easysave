using System.Diagnostics;
using System.Management;
using System.Security.Principal;

namespace EasyLib.Events;

public class ProcessStartEvent
{
    public static long NumberOfRunningProcesses;
    private readonly JobManager _jobManager;
    private readonly ManagementEventWatcher? _processStartEvent;
    private readonly ManagementEventWatcher? _processStopEvent;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// create the start and stop event for a process and bind the event to the job manager
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="jobManager"></param>
    public ProcessStartEvent(string? processName, JobManager jobManager)
    {
        // check if system is running Windows
        _jobManager = jobManager;
        if ((Environment.OSVersion.Platform == PlatformID.Win32NT) &&
            IsAdministrator) // only on Windows can run this feature
        {
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
#pragma warning disable CA1416
        }
        else
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var newThread = new Thread(() => _checkProcesses(processName));
            newThread.Start();
        }
    }

    public static bool IsAdministrator =>
        new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    private void ProcessStarted(object sender, EventArrivedEventArgs e)
    {
        if (NumberOfRunningProcesses == 0)
        {
            _jobManager.PauseAllJobs();
            Interlocked.Increment(ref NumberOfRunningProcesses);
        }
        else
        {
            Interlocked.Increment(ref NumberOfRunningProcesses);
        }
    }

    private void ProcessStopped(object sender, EventArrivedEventArgs e)
    {
        if (NumberOfRunningProcesses == 1)
        {
            _jobManager.ResumeAllJobs();
            Interlocked.Decrement(ref NumberOfRunningProcesses);
        }
        else
        {
            Interlocked.Decrement(ref NumberOfRunningProcesses);
        }
    }

    public void Stop()
    {
#pragma warning disable CA1416
        if (_processStartEvent != null && _processStopEvent != null)
        {
            _processStopEvent.Stop();
            _processStartEvent.Stop();
        }
        else
        {
            _cancellationTokenSource.Cancel();
        }
#pragma warning restore CA1416
    }

    private void _checkProcesses(string processName)
    {
        while (true)
        {
            if (Process.GetProcessesByName(processName).Length > Interlocked.Read(ref NumberOfRunningProcesses))
            {
                ProcessStarted(null, null);
            }
            else if (Process.GetProcessesByName(processName).Length < Interlocked.Read(ref NumberOfRunningProcesses))
            {
                ProcessStopped(null, null);
            }

            Thread.Sleep(100);
            if (_cancellationTokenSource.IsCancellationRequested) return;
        }
    }
}
