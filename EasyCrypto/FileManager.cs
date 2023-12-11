using System.Text;

namespace EasyCrypto;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files
/// </summary>
public class FileManager(string path, string key)
{
    private string FilePath { get; } = path;
    private string Key { get; } = key;

    /// <summary>
    /// check if the file exists
    /// </summary>
    private void CheckFile()
    {
        if (File.Exists(FilePath))
            return;

        Console.WriteLine("File not found.");
        Environment.Exit(1);
    }

    /// <summary>
    /// Encrypts the file with xor encryption
    /// </summary>
    public void TransformFile()
    {
        CheckFile();
        var fileBytes = File.ReadAllBytes(FilePath);
        var keyBytes = ConvertToByte(Key);
        fileBytes = XorMethod(fileBytes, keyBytes);
        File.WriteAllBytes(FilePath, fileBytes);
    }

    /// <summary>
    /// Convert a string in byte array
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static byte[] ConvertToByte(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// Compare the bytes of the file with the bytes of the key and create a new byte array using the xor method
    /// </summary>
    /// <param name="fileBytes">Bytes of the file to convert</param>
    /// <param name="keyBytes">Key to use</param>
    /// <returns>Bytes of the encrypted file</returns>
    private static byte[] XorMethod(IReadOnlyList<byte> fileBytes, IReadOnlyList<byte> keyBytes)
    {
        var result = new byte[fileBytes.Count];
        for (var i = 0; i < fileBytes.Count; i++)
        {
            result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
        }

        return result;
    }
}
