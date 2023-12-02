using EasyCLI.Commands.CommandBuilder;
using EasyCLI.Commands.CommandFeatures;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandBuilderTests;

public class CommandBuilderTests
{
    [Fact]
    public void SetName_ShouldSetName()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();

        // Act
        commandBuilder.SetName("test");

        // Assert
        Assert.Equal("test", commandBuilder.Name);
    }

    [Fact]
    public void SetDescription_ShouldSetDescription()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();

        // Act
        commandBuilder.SetDescription("test description");

        // Assert
        Assert.Equal("test description", commandBuilder.Description);
    }

    [Fact]
    public void SetAliases_ShouldSetAliases()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();
        var aliases = new List<string> { "alias1", "alias2" };

        // Act
        commandBuilder.SetAliases(aliases);

        // Assert
        Assert.Equal(aliases, commandBuilder.Aliases);
    }

    [Fact]
    public void AddArg_ShouldAddArg()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();
        var arg = new CommandArg();

        // Act
        commandBuilder.AddArg(arg);

        // Assert
        Assert.Contains(arg, commandBuilder.Args);
    }

    [Fact]
    public void AddFlag_ShouldAddFlag()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();
        var flag = new CommandFlag();

        // Act
        commandBuilder.AddFlag(flag);

        // Assert
        Assert.Contains(flag, commandBuilder.Flags);
    }

    [Fact]
    public void AddSubCommand_ShouldAddSubCommand()
    {
        // Arrange
        var commandBuilder = new CommandBuilder();
        var subCommand = new MockCommand();

        // Act
        commandBuilder.AddSubCommand(subCommand);

        // Assert
        Assert.Contains(subCommand, commandBuilder.SubCommands);
    }
}
