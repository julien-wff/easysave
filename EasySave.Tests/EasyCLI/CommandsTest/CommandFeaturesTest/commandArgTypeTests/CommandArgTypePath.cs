using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest.commandArgTypeTests;

public class CommandArgTypePathTests
{
    [Fact]
    public void Parse_ShouldReturnJobType()
    {
        // Arrange
        var input = Path.GetTempPath();
        var commandArgTypeJobType = new CommandArgTypePath { RawValue = input };

        // Act
        var result = commandArgTypeJobType.ParseValue();

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal(Path.GetTempPath(), result);
    }

    [Fact]
    public void ToString_ShouldValidateArgument()
    {
        // Arrange
        var input = Path.GetTempPath();
        var commandArgTypeJobType = new CommandArgTypePath { RawValue = input };

        // Act
        var result = commandArgTypeJobType.CheckValue();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldInvalidateArgument()
    {
        // Arrange
        const string input = "C:\n";
        var commandArgTypeJobType = new CommandArgTypePath { RawValue = input };

        // Act
        var result = commandArgTypeJobType.CheckValue();

        // Assert
        result.Should().BeFalse();
    }
}
