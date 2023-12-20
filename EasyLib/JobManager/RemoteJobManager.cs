using System.Net;
using System.Net.Sockets;
using EasyLib.Api;
using EasyLib.Enums;

namespace EasyLib.JobManager;

public class RemoteJobManager : JobManager
{
    private readonly Socket _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private JobManagerClient? _client;
    private Thread? _clientThread;
    public event EventHandler? JobListChanged;

    public bool Connect(EndPoint endPoint)
    {
        try
        {
            _clientSocket.Connect(endPoint);
        }
        catch (Exception)
        {
            return false;
        }

        _client = new JobManagerClient(this, _clientSocket);
        _clientThread = new Thread(_client.Listen);
        _clientThread.Start();

        return true;
    }

    public override void CleanStop()
    {
        _clientSocket.Close();
    }

    public override List<Job.Job> GetJobs()
    {
        return Jobs;
    }

    public override bool FetchJobs()
    {
        return false;
    }

    public void AddJob(Job.Job job)
    {
        Jobs.Add(job);
        JobListChanged?.Invoke(this, EventArgs.Empty);
    }

    public override JobCheckRule EditJob(Job.Job job, string name, string source, string destination, JobType? type)
    {
        throw new NotImplementedException();
    }

    public override JobCheckRule CheckJobRules(int id, string name, string source, string destination,
        bool testEmpty = true)
    {
        throw new NotImplementedException();
    }

    public override Job.Job CreateJob(string name, string src, string dest, JobType type)
    {
        throw new NotImplementedException();
    }

    public override void DeleteJob(Job.Job job)
    {
        throw new NotImplementedException();
    }

    public override void CancelJob(Job.Job job)
    {
        job.Cancel();
    }

    public override void PauseJob(Job.Job job)
    {
        job.Pause();
    }

    public override void ResumeJob(Job.Job job)
    {
        job.Resume();
    }

    public override void PauseAllJobs()
    {
        foreach (var job in Jobs)
        {
            job.Pause();
        }
    }

    public override void ResumeAllJobs()
    {
        foreach (var job in Jobs)
        {
            job.Resume();
        }
    }
}
