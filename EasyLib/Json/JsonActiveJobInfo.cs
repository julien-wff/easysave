// ReSharper disable InconsistentNaming

namespace EasyLib.Json;

/// <summary>
/// JSON representation of the part of a job that contains information about the active job.
/// </summary>
public struct JsonActiveJobInfo
{
    public uint total_file_count;
    public ulong total_file_size;
    public uint files_copied;
    public ulong bytes_copied;
    public string current_file_source;
    public string current_file_destination;
}
