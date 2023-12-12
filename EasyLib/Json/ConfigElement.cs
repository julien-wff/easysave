namespace EasyLib.Json;

public struct ConfigElement
{
    public string Language { get; init; }
    public List<string>? EncryptedFileTypes { get; init; }
    public string? XorKey { get; init; }
    private readonly string _logFormat;

    public string LogFormat
    {
        get => _logFormat;
        init => _logFormat = value is ".xml" or ".json" ? value : ".json";
    }

    public string? EasyCryptoPath { get; init; }
    public string? CompanySoftwareProcessPath { get; init; }
}
