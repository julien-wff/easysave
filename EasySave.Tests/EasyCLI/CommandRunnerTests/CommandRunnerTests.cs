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

    /*[Fact]
    public void RunWithArgs_ShouldCallRun_WhenCommandMatches()
    {
        // Arrange
        var commandRunner = new CommandRunner();
        var command = new Mock<MockCommand>();
        command.Setup(c => c.ValidateArgs(It.IsAny<IEnumerable<string>>())).Returns(true);
        commandRunner.RegisterCommand(command.Object);
        var args = new List<string> { "mock" };

        // Act
        var result = commandRunner.RunWithArgs(args);

        // Assert
        command.Verify(c => c.Run(It.IsAny<IEnumerable<string>>()), Times.Once);
        result.Should().BeTrue();
    }*/

    [Fact]
    public void GetCommandFromArgs_ShouldReturnNull_WhenNoCommandMatches()
    {
        // Arrange
        var commandRunner = CommandRunner.GetInstance();
        var args = new List<string> { "command" };

        // Act
        var result = commandRunner.GetCommandFromArgs(args);

        // Assert
        result.Should().BeNull();
    }

    /*[Fact]
    public void GetCommandFromArgs_ShouldReturnCommand_WhenCommandMatches()
    {
        // Arrange
        var commandRunner = new CommandRunner();
        var command = new Mock<MockCommand>();
        command.Setup(c => c.ValidateArgs(It.IsAny<IEnumerable<string>>())).Returns(true);
        commandRunner.RegisterCommand(command.Object);
        var args = new List<string> { "mock" };

        // Act
        var result = commandRunner.GetCommandFromArgs(args);

        // Assert
        result.Should().Be(command.Object);
    }*/
}
