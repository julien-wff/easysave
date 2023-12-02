using EasyCLI.Commands;
using EasyCLI.Commands.CommandBuilder;

namespace EasySave.Tests.EasyCLI.CommandsTest;

public class MockCommand : Command
{
    public override CommandBuilder Params { get; } = new CommandBuilder()
        .SetName("mock")
        .SetDescription("Mock command")
        .SetAliases(new[] { "mo", "m" });

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        return true;
    }

    public override void Run(IEnumerable<string> args)
    {
    }
}
