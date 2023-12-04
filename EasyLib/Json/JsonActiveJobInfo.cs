// ReSharper disable InconsistentNaming

namespace EasyLib.Json;

/// <summary>
/// JSON representation of the part of a job that contains information about the active job.
/// </summary>
public struct JsonActiveJobInfo
{
    public uint total_file_count { get; init; }
    public ulong total_file_size { get; init; }
    public uint files_copied { get; init; }
    public ulong bytes_copied { get; init; }
    public string current_file_source { get; init; }
    public string current_file_destination { get; init; }
}
