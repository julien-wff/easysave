namespace EasyCrypto;

public static class Program
{
    public static void Main(string[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        var fileManager = new FileManager(args[0], args[1]);
        fileManager.TransformFile();
    }
}
