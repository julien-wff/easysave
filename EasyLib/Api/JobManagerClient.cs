using System.Net.Sockets;
using System.Text;
using EasyLib.Enums;
using EasyLib.Job;
using EasyLib.JobManager;
using EasyLib.Json;
using Newtonsoft.Json;

namespace EasyLib.Api;

public class JobManagerClient(RemoteJobManager remoteJobManager, Socket serverSocket)
{
    public void Listen()
    {
        while (true)
        {
            try
            {
                var buffer = new byte[1024];
                var received = serverSocket.Receive(buffer, SocketFlags.None);
                if (received == 0)
                    break;

                var data = new byte[received];
                Array.Copy(buffer, data, received);

                var text = Encoding.ASCII.GetString(data);

                foreach (var line in text.Trim().Split("\n\r"))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    _handleAction(line);
                }
            }
            catch (Exception)
            {
                break;
            }
        }
    }

    private void _handleAction(string jsonAction)
    {
        var request = JsonConvert.DeserializeObject<JsonApiRequest>(jsonAction);
        var job = new RemoteJob(request.Job);

        switch (request.Action)
        {
            case ApiAction.Start:
                break;
            case ApiAction.Pause:
                break;
            case ApiAction.Resume:
                break;
            case ApiAction.Cancel:
                break;
            case ApiAction.Delete:
                break;
            case ApiAction.Edit:
                break;
            case ApiAction.Error:
                break;
            case ApiAction.State:
                break;
            case ApiAction.Progress:
                break;
            case ApiAction.Create:
                remoteJobManager.AddJob(job);
                break;
            default:
                return;
        }
    }
}
