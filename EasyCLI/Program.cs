using EasyCLI.Commands;

namespace EasyCLI;

public static class Program
{
    public static void Main(string[] args)
    {
        var version = new VersionCommand();
        version.ValidateArgs(new[] { "salut" });
        version.Run(args);
    }
}
