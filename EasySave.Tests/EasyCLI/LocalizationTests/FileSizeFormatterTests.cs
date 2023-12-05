using EasyCLI.Localization;

namespace EasySave.Tests.EasyCLI.LocalizationTests;

public class FileSizeFormatterTests
{
    [Fact]
    public void Format_ShouldReturnZeroBytes()
    {
        // Arrange
        const ulong bytes = 0;

        // Act
        var result = FileSizeFormatter.Format(bytes);

        // Assert
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void Format_ShouldReturnKilobytes()
    {
        // Arrange
        const ulong bytes = 1024;

        // Act
        var result = FileSizeFormatter.Format(bytes);

        // Assert
        Assert.Equal("1 KB", result);
    }

    [Fact]
    public void Format_ShouldReturnGigabytes()
    {
        // Arrange
        const ulong bytes = (ulong)(2.2555 * 1024 * 1024 * 1024);

        // Act
        var result = FileSizeFormatter.Format(bytes);

        // Assert
        Assert.Equal("2 GB", result);
    }

    [Fact]
    public void Format_ShouldReturnTerabytes()
    {
        // Arrange
        const ulong bytes = 1024ul * 1024ul * 1024ul * 1024ul * 1024ul;

        // Act
        var result = FileSizeFormatter.Format(bytes);

        // Assert
        Assert.Equal("1024 TB", result);
    }
}
