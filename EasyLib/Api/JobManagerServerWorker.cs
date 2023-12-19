using System.Net.Sockets;
using System.Text;
using EasyLib.Enums;
using EasyLib.Job;
using EasyLib.Json;

namespace EasyLib.Api;

public class Worker
{
    private readonly JobManagerServer _server;
    private readonly TcpClient _socket;
    private readonly Stream _stream;

    public Worker(TcpClient socket, JobManagerServer server)
    {
        this._socket = socket;
        this._stream = socket.GetStream();
        this._server = server;
    }

    public void Start()
    {
        new Thread(Run).Start();
    }

    public void Send(JsonApiRequest request)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        _stream.Write(buffer, 0, buffer.Length);
    }

    private void Run()
    {
        try
        {
            byte[] buffer = new byte[2018];
            while (true)
            {
                int receivedBytes = _stream.Read(buffer, 0, buffer.Length);
                if (receivedBytes < 1)
                    break;
                JsonApiRequest request =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<JsonApiRequest>(
                        Encoding.UTF8.GetString(buffer, 0, receivedBytes));
                _server.ExecuteJobCommand(request.Action, new LocalJob(request.Job));
                if (_server.CancellationTokenSource.IsCancellationRequested)
                    break;
            }
        }
        catch (Exception e)
        {
            Close();
            lock (_server.ServerLockObject)
            {
                _server.RemoveWorker(this);
            }
        }
    }

    public void SendAllJobs(List<Job.Job> jobs)
    {
        foreach (Job.Job job in jobs)
        {
            Send(new JsonApiRequest() { Action = ApiAction.Create, Job = job.ToJsonJob() });
        }
    }

    public void Close()
    {
        _stream.Close();
    }
}
