namespace EasyCrypto;

public static class Program
{
    public static void Main(string[] args)
    {
        var fileManager = new FileManager(args[0], "test");
        fileManager.EncryptFile();
    }
}
