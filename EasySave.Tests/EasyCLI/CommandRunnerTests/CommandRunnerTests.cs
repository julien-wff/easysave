using EasyCLI.CommandRunner;
using EasySave.Tests.EasyCLI.CommandsTest;

namespace EasySave.Tests.EasyCLI.CommandRunnerTests;

public class CommandRunnerTests
{
    [Fact]
    public void RegisterCommand_ShouldAddCommandToCommands()
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var command = new Mock<MockCommand>();

        // Act
        commandRunner.RegisterCommand(command.Object);

        // Assert
        CommandRunner.GetInstance().Commands.Should().Contain(command.Object);
    }

    [Fact]
    public void RunWithArgs_ShouldReturnFalse_WhenNoCommandMatches()
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var args = new List<string> { "command" };

        // Act
        var result = commandRunner.RunWithArgs(args);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RunWithArgs_ShouldCallRun_WhenCommandNameMatches()
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var command = new MockCommand();
        commandRunner.RegisterCommand(command);
        var args = new List<string> { "mock" };

        // Act
        var result = commandRunner.RunWithArgs(args);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("mock")]
    [InlineData("mo")]
    [InlineData("m")]
    public void GetCommandFromArgs_ShouldReturnCommand_WhenCommandMatches(string input)
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var command = new MockCommand();
        commandRunner.RegisterCommand(command);
        var args = new List<string> { input };

        // Act
        var result = commandRunner.GetCommandFromArgs(args);

        // Assert
        command.Params.Name.Should().Be(result?.Params.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("moc")]
    [InlineData("mockk")]
    public void GetCommandFromArgs_ShouldReturnNull_WhenNoCommandMatches(string? input)
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var command = new MockCommand();
        commandRunner.RegisterCommand(command);
        var args = new List<string>();
        if (input != null)
        {
            args.Add(input);
        }

        // Act
        var result = commandRunner.GetCommandFromArgs(args);

        // Assert
        result.Should().BeNull();
    }
}
