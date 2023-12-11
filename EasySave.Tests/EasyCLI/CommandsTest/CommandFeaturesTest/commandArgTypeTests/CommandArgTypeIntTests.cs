using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest.commandArgTypeTests;

public class CommandArgTypeIntTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("1234", 1234)]
    [InlineData("-1", -1)]
    public void ParseValue_ShouldReturnInt(string input, int output)
    {
        // Arrange
        var commandArgTypeInt = new CommandArgTypeInt { RawValue = input };

        // Act
        var result = commandArgTypeInt.ParseValue();

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(result, output);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1234")]
    [InlineData("-1")]
    public void CheckValue_ShouldReturnTrue(string input)
    {
        // Arrange
        var commandArgTypeInt = new CommandArgTypeInt { RawValue = input };

        // Act
        var result = commandArgTypeInt.CheckValue();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("1.1")]
    [InlineData("1234.1")]
    [InlineData("-1.1")]
    [InlineData("a")]
    [InlineData("a1")]
    [InlineData("1a")]
    [InlineData("one")]
    public void CheckValue_ShouldReturnFalse(string input)
    {
        // Arrange
        var commandArgTypeInt = new CommandArgTypeInt { RawValue = input };

        // Act
        var result = commandArgTypeInt.CheckValue();

        // Assert
        result.Should().BeFalse();
    }
}
