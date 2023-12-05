using EasyCLI;
using EasyCLI.CommandRunner;

namespace EasySave.Tests.EasyCLI;

public class ProgramTests
{
    [Fact]
    public void Main_ShouldRegisterCommands()
    {
        // Arrange

        // Act
        Program.Main(Array.Empty<string>());

        // Assert
        CommandRunner.GetInstance().Commands.Should().NotBeEmpty();
    }
}
