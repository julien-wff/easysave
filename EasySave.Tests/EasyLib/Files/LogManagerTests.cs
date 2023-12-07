using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files;

public class LogManagerTests
{
    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        // Arrange

        // Act
        var instance1 = LogManager.Instance;
        var instance2 = LogManager.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }
}
