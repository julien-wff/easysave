// ReSharper disable InconsistentNaming

namespace EasyLib.Json;

/// <summary>
/// JSON representation of a job.
/// </summary>
public struct JsonJob
{
    public uint id;
    public string name;
    public string source_folder;
    public string destination_folder;
    public string type;
    public string state;
    public JsonActiveJobInfo? active_job_info;
}
