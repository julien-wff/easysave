using EasyCLI.Commands;

namespace EasyCLI;

public static class Program
{
    public static void Main(string[] args)
    {
        CommandRunner
            .CommandRunner
            .GetInstance()
            .RegisterCommand(new VersionCommand())
            .RunWithArgs(args);
    }
}
