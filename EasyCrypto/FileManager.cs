using System.Text;

namespace EasyCrypto;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files
/// </summary>
public class FileManager
{
    public FileManager(string path, string key)
    {
        FilePath = path;
        Key = key;
    }

    public string FilePath { get; }
    public string Key { get; }

    /// <summary>
    /// check if the file exists
    /// </summary>
    private void checkFile()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("File not found.");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Encrypts the file with xor encryption
    /// </summary>
    public void EncryptFile()
    {
        checkFile();
        var fileBytes = File.ReadAllBytes(FilePath);
        var keyBytes = ConvertToByte(Key);
        fileBytes = xorMethod(fileBytes, keyBytes);
        File.WriteAllBytes(FilePath, fileBytes);
    }

    /// <summary>
    /// Convert a string in byte array
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private byte[] ConvertToByte(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// Compare the bytes of the file with the bytes of the key and create a new byte array using the xor method
    /// </summary>
    /// <param name="fileBytes"></param>
    /// <param name="keyBytes"></param>
    /// <returns></returns>
    private byte[] xorMethod(byte[] fileBytes, byte[] keyBytes)
    {
        var result = new byte[fileBytes.Length];
        for (var i = 0; i < fileBytes.Length; i++)
        {
            result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return result;
    }
}
