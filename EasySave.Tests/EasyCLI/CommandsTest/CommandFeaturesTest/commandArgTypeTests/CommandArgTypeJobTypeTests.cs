using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyLib.Enums;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest.commandArgTypeTests;

public class CommandArgTypeJobTypeTests
{
    [Fact]
    public void Parse_ShouldReturnJobType()
    {
        // Arrange
        const string input = "Full";
        var commandArgTypeJobType = new CommandArgTypeJobType { RawValue = input };

        // Act
        var result = commandArgTypeJobType.ParseValue();

        // Assert
        Assert.IsType<JobType>(result);
        Assert.Equal(JobType.Full, result);
    }

    [Fact]
    public void ToString_ShouldValidateArgument()
    {
        // Arrange
        const string input = "Full";
        var commandArgTypeJobType = new CommandArgTypeJobType { RawValue = input };

        // Act
        var result = commandArgTypeJobType.CheckValue();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldInvalidateArgument()
    {
        // Arrange
        const string input = "xxx-Invalid-Enum-xxx";
        var commandArgTypeJobType = new CommandArgTypeJobType { RawValue = input };

        // Act
        var result = commandArgTypeJobType.CheckValue();

        // Assert
        result.Should().BeFalse();
    }
}
