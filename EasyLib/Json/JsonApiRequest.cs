using EasyLib.Enums;

namespace EasyLib.Json;

public readonly struct JsonApiRequest(ApiAction action, JsonJob job)
{
    public JsonApiRequest(ApiAction action, Job.Job job) : this(action, job.ToJsonJob())
    {
    }

    public ApiAction Action { get; init; } = action;
    public JsonJob Job { get; init; } = job;
}
