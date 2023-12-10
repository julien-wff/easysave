﻿namespace EasyLib.Json;

public struct JsonConfig
{
    public List<string>? CryptedFileTypes { get; init; }
    public List<string>? BusinnesProcesses { get; init; }
    public string? XorKey { get; init; }
    private readonly string _logFormat;

    public string LogFormat
    {
        get => _logFormat;
        init
        {
            switch (value)
            {
                case "xml":
                    _logFormat = "xml";
                    break;
                default:
                    _logFormat = "json";
                    break;
            }
        }
    }

    public string? EasyCryptoPath { get; init; }
}
