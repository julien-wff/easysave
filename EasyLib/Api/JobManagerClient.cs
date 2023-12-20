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
                var buffer = new byte[2018];
                var receivedBytes = serverSocket.Receive(buffer);

                if (receivedBytes < 1)
                    break;

                var json = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                _handleAction(json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                break;
            }
        }
    }

    public bool SendAction(ApiAction action, Job.Job job)
    {
        try
        {
            var json = JsonConvert.SerializeObject(new JsonApiRequest(action, job, job.CurrentlyRunning)) + "\n\r";
            var data = Encoding.ASCII.GetBytes(json);
            serverSocket.Send(data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void _handleAction(string jsonAction)
    {
        JsonApiRequest request;
        try
        {
            request = JsonConvert.DeserializeObject<JsonApiRequest>(jsonAction);
        }
        catch (Exception)
        {
            return;
        }

        var job = _createOrUpdateJob(request.Job, request.JobRunning);

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
                job.OnJobError(new ApplicationException("Unknown error"));
                break;
            case ApiAction.State:
                job.OnJobStateChange(job.State, job);
                break;
            case ApiAction.Progress:
                job.OnJobProgress(job);
                break;
            case ApiAction.Create:
                remoteJobManager.AddJob(job);
                break;
            default:
                return;
        }
    }

    private Job.Job _createOrUpdateJob(JsonJob jsonJob, bool running)
    {
        var job = remoteJobManager.GetJobs().Find(j => j.Id == jsonJob.id)
                  ?? new RemoteJob(jsonJob, this);
        job.Name = jsonJob.name;
        job.SourceFolder = jsonJob.source_folder;
        job.DestinationFolder = jsonJob.destination_folder;
        job.Type = EnumConverter<JobType>.ConvertToEnum(jsonJob.type);
        job.State = EnumConverter<JobState>.ConvertToEnum(jsonJob.state);
        job.FilesCount = jsonJob.active_job_info?.total_file_count ?? 0;
        job.FilesSizeBytes = jsonJob.active_job_info?.total_file_size ?? 0;
        job.FilesCopied = jsonJob.active_job_info?.files_copied ?? 0;
        job.FilesBytesCopied = jsonJob.active_job_info?.bytes_copied ?? 0;
        job.CurrentFileSource = jsonJob.active_job_info?.current_file_source ?? string.Empty;
        job.CurrentFileDestination = jsonJob.active_job_info?.current_file_destination ?? string.Empty;
        job.CurrentlyRunning = running;
        return job;
    }
}
