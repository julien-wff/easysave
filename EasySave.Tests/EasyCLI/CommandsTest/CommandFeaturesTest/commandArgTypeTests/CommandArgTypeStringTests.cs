using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest.commandArgTypeTests;

public class CommandArgTypeStringTests
{
    [Fact]
    public void Parse_ShouldReturnString()
    {
        // Arrange
        var input = "test";
        var commandArgTypeString = new CommandArgTypeString { RawValue = input };

        // Act
        var result = commandArgTypeString.ParseValue();

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal(input, result);
    }

    [Fact]
    public void ToString_ShouldValidateArgument()
    {
        // Arrange
        var input = "test";
        var commandArgTypeString = new CommandArgTypeString { RawValue = input };

        // Act
        var result = commandArgTypeString.CheckValue();

        // Assert
        result.Should().BeTrue();
    }
}
