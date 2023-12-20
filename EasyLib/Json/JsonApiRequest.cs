using EasyLib.Enums;

namespace EasyLib.Json;

public readonly struct JsonApiRequest(ApiAction action, JsonJob job, bool running)
{
    public JsonApiRequest(ApiAction action, Job.Job job, bool running) : this(action, job.ToJsonJob(), running)
    {
    }

    public ApiAction Action { get; init; } = action;
    public JsonJob Job { get; init; } = job;
    public bool JobRunning { get; init; } = running;
}
