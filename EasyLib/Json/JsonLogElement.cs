namespace EasyLib.Json;

public struct JsonLogElement
{
    public string JobName { get; set; }
    public int TransferTime { get; set; }
    public string SourcePath { get; set; }
    public string DestinationPath { get; set; }
    public ulong FileSize { get; set; }
}
