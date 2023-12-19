using System.Net;
using System.Net.Sockets;
using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.JobManager;
using EasyLib.Json;

namespace EasyLib.Api;

public class JobManagerServer
{
    private static readonly object LockObject = new object();
    private readonly LocalJobManager _localJobManager;
    private readonly TcpListener _serverSocket = null!;
    private readonly List<Worker> _workers = new List<Worker>();

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

    public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

    private void _waitForConnection()
    {
        try
        {
            while (true)
            {
                TcpClient socket = _serverSocket.AcceptTcpClient();
                Worker worker = new Worker(socket, this);
                AddWorker(worker);
                if (CancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }
        catch (SocketException e)
        {
            CleanInstance();
        }
    }

    private void AddWorker(Worker worker)
    {
        lock (LockObject)
        {
            _workers.Add(worker);
            worker.Start();
        }
    }

    public void RemoveWorker(Worker worker)
    {
        lock (LockObject)
        {
            _workers.Remove(worker);
            worker.Close();
        }
    }

    public void Broadcast(ApiAction action, JsonJob jsonJob)
    {
        lock (LockObject)
        {
            foreach (Worker worker in _workers)
            {
                worker.Send(new JsonApiRequest() { Action = action, Job = jsonJob });
            }
        }
    }

    public void ExecuteJobCommand(ApiAction action, Job.Job job)
    {
        switch (action)
        {
            case ApiAction.Start:
                var localJob = _localJobManager.GetJobsFromIds(new List<uint>() { job.Id });
                if (localJob.Count == 1)
                {
                    _localJobManager.ExecuteJob(localJob[0]);
                }

                break;
            case ApiAction.Cancel:
                break;
            case ApiAction.Pause:
                break;
            case ApiAction.Edit:
                break;
            case ApiAction.Delete:
                break;
            case ApiAction.Resume:
                break;
        }
    }

    public void CleanInstance()
    {
        var numWorkers = _workers.Count;
        for (var i = 0; i < numWorkers; i++)
        {
            RemoveWorker(_workers[0]);
        }

        _serverSocket.Stop();
    }
}
