using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files;

public class JsonFileUtilsTest
{
    private const string PersonString = """
                                        {
                                          "Name": "John Doe",
                                          "Age": 42,
                                          "Children": [
                                            {
                                              "Name": "Jane Doe",
                                              "Age": 13,
                                              "Children": []
                                            }
                                          ]
                                        }
                                        """;

    private readonly Person _personStruct = new()
    {
        Name = "John Doe",
        Age = 42,
        Children = new List<Person>
        {
            new()
            {
                Name = "Jane Doe",
                Age = 13,
                Children = new List<Person>()
            }
        }
    };

    [Fact]
    public void WriteJson_ShouldWriteFormattedJson()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.Delete(path);

        // Act
        JsonFileUtils.WriteJson(path, _personStruct);

        // Assert
        var actual = File.ReadAllText(path);
        Assert.StartsWith(PersonString, actual);
    }

    [Fact]
    public void ReadJson_ShouldReadFormattedJson()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.WriteAllText(path, PersonString);

        // Act
        var actual = JsonFileUtils.ReadJson<Person>(path);

        // Assert
        actual.Name.Should().Be(_personStruct.Name);
        actual.Age.Should().Be(_personStruct.Age);
        actual.Children.Count.Should().Be(_personStruct.Children.Count);
        actual.Children[0].Name.Should().Be(_personStruct.Children[0].Name);
        actual.Children[0].Age.Should().Be(_personStruct.Children[0].Age);
        actual.Children[0].Children.Count.Should().Be(_personStruct.Children[0].Children.Count);
    }

    [Fact]
    public void AppendJsonToList_ShouldAppendFormattedJson()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.Delete(path);
        var expectedObject = new List<Person> { _personStruct, _personStruct, _personStruct };

        // Act
        var results = new List<bool>();
        for (var i = 0; i < 3; i++)
        {
            var res = JsonFileUtils.AppendJsonToList(path, _personStruct);
            results.Add(res);
        }

        // Assert
        var actual = JsonFileUtils.ReadJson<List<Person>>(path);
        actual.Should().NotBeNullOrEmpty();
        results.Should().AllBeEquivalentTo(true);
        actual!.Count.Should().Be(expectedObject.Count);
        actual[2].Name.Should().Be(expectedObject[2].Name);
        actual[2].Children.Count.Should().Be(expectedObject[2].Children.Count);
        actual[2].Children[0].Name.Should().Be(expectedObject[2].Children[0].Name);
    }

    [Fact]
    public void AppendJsonToList_ShouldReturnFalseOnInvalidJson()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "test");

        // Act
        var result = JsonFileUtils.AppendJsonToList(path, _personStruct);

        // Assert
        var actual = File.ReadAllText(path);
        result.Should().BeFalse();
        actual.Should().Be("test");
    }

    private struct Person
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public List<Person> Children { get; init; }
    }
}
