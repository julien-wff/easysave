using EasyLib.Enums;

namespace EasySave.Tests.EasyLib.EnumsTests;

public class EnumConverterTests
{
    [Fact]
    public void ConvertToEnum_ValidString_ReturnsCorrectEnum()
    {
        // Arrange
        const string enumValueStr = "Full";

        // Act
        var result = EnumConverter<JobType>.ConvertToEnum(enumValueStr);

        // Assert
        Assert.Equal(JobType.Full, result);
    }

    [Fact]
    public void ConvertToEnum_InvalidString_ThrowsArgumentException()
    {
        // Arrange
        const string enumValueStr = "Invalid";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EnumConverter<JobType>.ConvertToEnum(enumValueStr));
    }

    [Fact]
    public void ConvertToString_ValidEnum_ReturnsCorrectString()
    {
        // Arrange
        const JobType enumValue = JobType.Full;

        // Act
        var result = EnumConverter<JobType>.ConvertToString(enumValue);

        // Assert
        Assert.Equal("Full", result);
    }

    [Fact]
    public void ConvertToString_InvalidEnum_ThrowsArgumentException()
    {
        // Arrange
        const JobType enumValue = (JobType)999; // Invalid enum value

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EnumConverter<JobType>.ConvertToString(enumValue));
    }
}
