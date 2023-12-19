using System.Net.Sockets;
using System.Text;
using EasyLib.Enums;
using EasyLib.Job;
using EasyLib.Json;

namespace EasyLib.Api;

public class Worker(TcpClient socket, JobManagerServer server)
{
    private readonly Stream _stream = socket.GetStream();

    public void Start()
    {
        new Thread(Run).Start();
    }

    public void Send(JsonApiRequest request)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(request) + "\n\r";
        var buffer = Encoding.UTF8.GetBytes(json);
        _stream.Write(buffer, 0, buffer.Length);
    }

    private void Run()
    {
        try
        {
            var buffer = new byte[2018];
            while (true)
            {
                var receivedBytes = _stream.Read(buffer, 0, buffer.Length);
                if (receivedBytes < 1)
                    break;
                var request =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<JsonApiRequest>(
                        Encoding.UTF8.GetString(buffer, 0, receivedBytes));
                server.ExecuteJobCommand(request.Action, new LocalJob(request.Job));
                if (server.CancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }
        catch (Exception)
        {
            Close();
            lock (server.ServerLockObject)
            {
                server.RemoveWorker(this);
            }
        }
    }

    public void SendAllJobs(List<Job.Job> jobs)
    {
        foreach (var job in jobs)
        {
            Send(new JsonApiRequest() { Action = ApiAction.Create, Job = job.ToJsonJob() });
        }
    }

    public void Close()
    {
        _stream.Close();
    }
}
