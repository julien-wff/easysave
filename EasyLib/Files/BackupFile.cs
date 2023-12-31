﻿using System.Security.Cryptography;

namespace EasyLib.Files;

/// <summary>
/// Stores information about the files that are backed up.
/// </summary>
public class BackupFile
{
    public readonly string Extension;
    public readonly string Hash;
    public readonly string Name;
    public readonly ulong Size;

    public BackupFile(string path)
    {
        var fileInfo = new FileInfo(path);
        Name = fileInfo.Name;
        Size = (ulong)fileInfo.Length;
        Hash = _calculateChecksum(path);
        Extension = fileInfo.Extension;
    }

    private static string _calculateChecksum(string path)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path);
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
    }
}
