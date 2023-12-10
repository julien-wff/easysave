namespace EasyCrypto;

public static class Program
{
    public static void Main(string[] args)
    {
        var fileManager = new FileManager(args[0], args[1]);
        fileManager.TransformFile();
    }
}
