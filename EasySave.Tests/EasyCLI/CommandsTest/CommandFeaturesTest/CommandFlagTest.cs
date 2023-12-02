using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest;

public class CommandFlagTests
{
    [Fact]
    public void SetName_ShouldSetCommandFlagName()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedName = "testName";

        // Act
        commandFlag.SetName(expectedName);

        // Assert
        Assert.Equal(expectedName, commandFlag.Name);
    }

    [Fact]
    public void SetDescription_ShouldSetCommandFlagDescription()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedDescription = "testDescription";

        // Act
        commandFlag.SetDescription(expectedDescription);

        // Assert
        Assert.Equal(expectedDescription, commandFlag.Description);
    }

    [Fact]
    public void SetType_ShouldSetCommandFlagType()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedType = new CommandArgTypeString();

        // Act
        commandFlag.SetType(expectedType);

        // Assert
        Assert.Equal(expectedType, commandFlag.Type);
    }

    [Fact]
    public void SetRequired_ShouldSetCommandFlagRequired()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedRequired = true;

        // Act
        commandFlag.SetRequired(expectedRequired);

        // Assert
        Assert.Equal(expectedRequired, commandFlag.Required);
    }

    [Fact]
    public void SetDefault_ShouldSetCommandFlagDefault()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedDefault = "defaultValue";

        // Act
        commandFlag.SetDefault(expectedDefault);

        // Assert
        Assert.Equal(expectedDefault, commandFlag.Default);
    }

    [Fact]
    public void SetAliases_ShouldSetCommandFlagAliases()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedAliases = new List<string> { "alias1", "alias2" };

        // Act
        commandFlag.SetAliases(expectedAliases);

        // Assert
        Assert.Equal(expectedAliases, commandFlag.Aliases);
    }

    [Fact]
    public void SetShortHands_ShouldSetCommandFlagShortHands()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedShortHands = new List<string> { "s1", "s2" };

        // Act
        commandFlag.SetShortHands(expectedShortHands);

        // Assert
        Assert.Equal(expectedShortHands, commandFlag.ShortHands);
    }

    [Fact]
    public void SetAllProperties_ShouldSetAllCommandFlagProperties()
    {
        // Arrange
        var commandFlag = new CommandFlag();
        var expectedName = "testName";
        var expectedDescription = "testDescription";
        var expectedType = new CommandArgTypeString();
        var expectedRequired = true;
        var expectedDefault = "defaultValue";
        var expectedAliases = new List<string> { "alias1", "alias2" };
        var expectedShortHands = new List<string> { "s1", "s2" };

        // Act
        commandFlag.SetName(expectedName)
            .SetDescription(expectedDescription)
            .SetType(expectedType)
            .SetRequired(expectedRequired)
            .SetDefault(expectedDefault)
            .SetAliases(expectedAliases)
            .SetShortHands(expectedShortHands);

        // Assert
        Assert.Equal(expectedName, commandFlag.Name);
        Assert.Equal(expectedDescription, commandFlag.Description);
        Assert.Equal(expectedType, commandFlag.Type);
        Assert.Equal(expectedRequired, commandFlag.Required);
        Assert.Equal(expectedDefault, commandFlag.Default);
        Assert.Equal(expectedAliases, commandFlag.Aliases);
        Assert.Equal(expectedShortHands, commandFlag.ShortHands);
    }
}
