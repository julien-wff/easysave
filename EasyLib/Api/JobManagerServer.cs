using System.Net;
using System.Net.Sockets;
using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.JobManager;
using EasyLib.Json;

namespace EasyLib.Api;

public class JobManagerServer
{
    private readonly LocalJobManager _localJobManager;
    private readonly TcpListener _serverSocket = null!;
    private readonly List<Worker> _workers = new List<Worker>();
    public readonly object ServerLockObject = new object();

    public JobManagerServer(LocalJobManager localJobManager)
    {
        _localJobManager = localJobManager;
        var serverPort = ConfigManager.Instance.ServerPort;
        if (serverPort != null)
        {
            _serverSocket = new TcpListener(IPAddress.Any, serverPort.Value);
            _serverSocket.Start();
            new Thread(_waitForConnection).Start();
        }
    }

    public CancellationTokenSource CancellationTokenSource { get; } = new();

    private void _waitForConnection()
    {
        try
        {
            while (true)
            {
                var socket = _serverSocket.AcceptTcpClient();

                var worker = new Worker(socket, this);
                AddWorker(worker);
                worker.SendAllJobs(_localJobManager.GetJobs());
                if (CancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }
        catch (SocketException)
        {
            CleanInstance();
        }
    }

    private void AddWorker(Worker worker)
    {
        lock (ServerLockObject)
        {
            _workers.Add(worker);
            worker.Start();
        }
    }

    public void RemoveWorker(Worker worker)
    {
        lock (ServerLockObject)
        {
            _workers.Remove(worker);
            worker.Close();
        }
    }

    public void Broadcast(ApiAction action, JsonJob jsonJob, bool jobRunning)
    {
        lock (ServerLockObject)
        {
            foreach (var worker in _workers)
            {
                worker.Send(new JsonApiRequest
                {
                    Action = action,
                    Job = jsonJob,
                    JobRunning = jobRunning
                });
            }
        }
    }

    public void ExecuteJobCommand(ApiAction action, Job.Job job)
    {
        var localJob = _localJobManager.GetJobsFromIds(new[] { (int)job.Id });
        if (localJob.Count != 1)
            return;
        switch (action)
        {
            case ApiAction.Start:
                _localJobManager.ExecuteJob(localJob[0]);
                break;
            case ApiAction.Cancel:
                _localJobManager.CancelJob(localJob[0]);
                break;
            case ApiAction.Pause:
                _localJobManager.PauseJob(localJob[0]);
                break;
            case ApiAction.Edit:
                _localJobManager.EditJob(localJob[0], job.Name, job.SourceFolder, job.DestinationFolder, job.Type);
                break;
            case ApiAction.Delete:
                _localJobManager.DeleteJob(localJob[0]);
                break;
            case ApiAction.Resume:
                _localJobManager.ResumeJob(localJob[0]);
                break;
            case ApiAction.Create:
                _localJobManager.CreateJob(job.Name, job.SourceFolder, job.DestinationFolder, job.Type);
                break;
        }
    }

    public void CleanInstance()
    {
        var numWorkers = _workers.Count;
        for (var i = 0; i < numWorkers; i++)
        {
            lock (ServerLockObject)
            {
                _workers[i].Close();
            }
        }

        _workers.Clear();
        _serverSocket.Stop();
    }
}
