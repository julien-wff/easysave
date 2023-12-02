using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasySave.Tests.EasyCLI.CommandsTest.CommandFeaturesTest
{
    public class CommandArgTests
    {
        [Fact]
        public void SetName_ShouldSetCommandFlagName()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedName = "testName";

            // Act
            commandArg.SetName(expectedName);

            // Assert
            Assert.Equal(expectedName, commandArg.Name);
        }

        [Fact]
        public void SetDescription_ShouldSetCommandFlagDescription()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedDescription = "testDescription";

            // Act
            commandArg.SetDescription(expectedDescription);

            // Assert
            Assert.Equal(expectedDescription, commandArg.Description);
        }

        [Fact]
        public void SetType_ShouldSetCommandFlagType()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedType = new CommandArgTypeString();

            // Act
            commandArg.SetType(expectedType);

            // Assert
            Assert.Equal(expectedType, commandArg.Type);
        }

        [Fact]
        public void SetRequired_ShouldSetCommandFlagRequired()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedRequired = true;

            // Act
            commandArg.SetRequired(expectedRequired);

            // Assert
            Assert.Equal(expectedRequired, commandArg.Required);
        }

        [Fact]
        public void SetDefault_ShouldSetCommandFlagDefault()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedDefault = "defaultValue";

            // Act
            commandArg.SetDefault(expectedDefault);

            // Assert
            Assert.Equal(expectedDefault, commandArg.Default);
        }

        [Fact]
        public void SetAllProperties_ShouldSetAllCommandFlagProperties()
        {
            // Arrange
            var commandArg = new CommandArg();
            var expectedName = "testName";
            var expectedDescription = "testDescription";
            var expectedType = new CommandArgTypeString();
            var expectedRequired = true;
            var expectedDefault = "defaultValue";

            // Act
            commandArg.SetName(expectedName)
                .SetDescription(expectedDescription)
                .SetType(expectedType)
                .SetRequired(expectedRequired)
                .SetDefault(expectedDefault);

            // Assert
            Assert.Equal(expectedName, commandArg.Name);
            Assert.Equal(expectedDescription, commandArg.Description);
            Assert.Equal(expectedType, commandArg.Type);
            Assert.Equal(expectedRequired, commandArg.Required);
            Assert.Equal(expectedDefault, commandArg.Default);
        }
    }
}
