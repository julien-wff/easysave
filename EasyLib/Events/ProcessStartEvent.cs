using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Security.Principal;

namespace EasyLib.Events;

[SuppressMessage("Interoperability", "CA1416")]
public class ProcessStartEvent
{
    private static long _numberOfRunningProcesses;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly JobManager.JobManager _jobManager;
    private readonly ManagementEventWatcher? _processStartEvent;
    private readonly ManagementEventWatcher? _processStopEvent;

    /// <summary>
    /// Create the start and stop event for a process and bind the event to the job manager
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="jobManager"></param>
    public ProcessStartEvent(string processName, JobManager.JobManager jobManager)
    {
        // check if system is running Windows
        _jobManager = jobManager;

        // only on Windows can run this feature
        if (Environment.OSVersion.Platform == PlatformID.Win32NT && IsAdministrator)
        {
            var eventQuery = new WqlEventQuery(
                $"SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName = '{processName}'"
            );

            _processStartEvent = new ManagementEventWatcher(eventQuery);
            _processStartEvent.EventArrived += ProcessStarted;
            // on stop
            _processStopEvent = new ManagementEventWatcher(eventQuery);
            _processStopEvent.EventArrived += ProcessStopped;
            _processStartEvent.Start();
            _processStopEvent.Start();
        }
        else
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            var newThread = new Thread(() => _checkProcesses(processName));
            newThread.Start();
        }
    }

    private static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent())
        .IsInRole(WindowsBuiltInRole.Administrator);

    private void ProcessStarted(object? sender = null, EventArrivedEventArgs? e = null)
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

    private void ProcessStopped(object? sender = null, EventArrivedEventArgs? e = null)
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
            var processesLength = Process.GetProcessesByName(processName).Length;
            if (processesLength > Interlocked.Read(ref _numberOfRunningProcesses))
            {
                ProcessStarted();
            }
            else if (processesLength < Interlocked.Read(ref _numberOfRunningProcesses))
            {
                ProcessStopped();
            }

            Thread.Sleep(500);
            if (_cancellationTokenSource.IsCancellationRequested) return;
        }
    }
}
