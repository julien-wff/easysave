// ReSharper disable InconsistentNaming

namespace EasyLib.Json;

/// <summary>
/// JSON representation of a job.
/// </summary>
public struct JsonJob
{
    public uint id { get; init; }
    public string name { get; init; }
    public string source_folder { get; init; }
    public string destination_folder { get; init; }
    public string type { get; init; }
    public string state { get; init; }
    public JsonActiveJobInfo? active_job_info { get; init; }
}
