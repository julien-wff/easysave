using EasyLib.Api;
using EasyLib.Enums;
using EasyLib.Json;

namespace EasyLib.Job;

public class RemoteJob(string name, string source, string destination, JobType type, JobManagerClient client)
    : Job(name, source, destination, type)
{
    /// <summary>
    /// Create a job instance from a JsonJob object
    /// </summary>
    /// <param name="job">JsonJob object</param>
    /// <param name="client">JobManagerClient instance</param>
    public RemoteJob(JsonJob job, JobManagerClient client)
        : this(job.name, job.source_folder, job.destination_folder, JobType.Full, client)
    {
        Id = job.id;
        Type = EnumConverter<JobType>.ConvertToEnum(job.type);
        State = EnumConverter<JobState>.ConvertToEnum(job.state);
        FilesCount = job.active_job_info?.total_file_count ?? 0;
        FilesSizeBytes = job.active_job_info?.total_file_size ?? 0;
        FilesCopied = job.active_job_info?.files_copied ?? 0;
        FilesBytesCopied = job.active_job_info?.bytes_copied ?? 0;
        CurrentFileSource = job.active_job_info?.current_file_source ?? string.Empty;
        CurrentFileDestination = job.active_job_info?.current_file_destination ?? string.Empty;
    }

    public override bool Resume()
    {
        return client.SendAction(ApiAction.Resume, this);
    }

    public override bool Run()
    {
        return client.SendAction(ApiAction.Start, this);
    }

    public override bool Pause()
    {
        return client.SendAction(ApiAction.Pause, this);
    }

    public override bool Cancel()
    {
        return client.SendAction(ApiAction.Cancel, this);
    }
}
