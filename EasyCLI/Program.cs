﻿using EasyCLI.Commands;

namespace EasyCLI;

public static class Program
{
    public static void Main(string[] args)
    {
        CommandRunner
            .CommandRunner
            .GetInstance()
            .RegisterCommand(new HelpCommand())
            .RegisterCommand(new VersionCommand())
            .RegisterCommand(new ListCommand())
            .RegisterCommand(new CheckCommand())
            .RegisterCommand(new CreateCommand())
            .RegisterCommand(new EditCommand())
            .RegisterCommand(new DeleteCommand())
            .RegisterCommand(new RunCommand())
            .RegisterCommand(new DiscardCommand())
            .RegisterCommand(new ConfigCommand())
            .RegisterCommand(new ResumeCommand())
            .RunWithArgs(args);
    }
}
