using System.Text;
using Newtonsoft.Json;

namespace EasyLib.Files;

/// <summary>
/// Utils class to manage content of JSON files.
/// </summary>
public static class JsonFileUtils
{
    private static readonly object _jsonLock = new();

    /// <summary>
    /// Enable formatting of JSON files.
    /// </summary>
    private static void SetDefaultSettings()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };
    }

    /// <summary>
    /// Read the content of a JSON file and deserialize it.
    /// </summary>
    /// <param name="path">Full path of the file</param>
    /// <typeparam name="T">Type of the resulting object</typeparam>
    /// <returns>A deserialized instance of the object</returns>
    public static T? ReadJson<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// Write an object to a JSON file. The file is overwritten if it already exists.
    /// </summary>
    /// <param name="path">Full path of the file</param>
    /// <param name="obj">Instance of the object to write</param>
    /// <typeparam name="T">Type of the object to write</typeparam>
    public static void WriteJson<T>(string path, T obj)
    {
        SetDefaultSettings();
        lock (_jsonLock)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, json);
        }
    }

    /// <summary>
    /// Append a JSON object to a JSON list. The file is created if it does not exist.
    /// Note: the usage of this class is very strict. The file must be a JSON array,
    /// and the written object must be a JSON object, not a list or a primitive.
    /// </summary>
    /// <param name="path">Full path of the file</param>
    /// <param name="obj">Instance of the object to write</param>
    /// <typeparam name="T">Type of the object to write</typeparam>
    /// <returns>True if the operation is successful, false otherwise</returns>
    public static bool AppendJsonToList<T>(string path, T obj)
    {
        SetDefaultSettings();

        if (!File.Exists(path) || new FileInfo(path).Length <= 3)
        {
            WriteJson(path, new List<T> { obj });
            return true;
        }

        try
        {
            // Read last line of file
            using var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(-1, SeekOrigin.End);

            // Open JSON list
            var foundCurlyBracket = false;
            while (fs.Position > 0 && !foundCurlyBracket)
            {
                var c = (char)fs.ReadByte();

                switch (c)
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                    case ']':
                        fs.SetLength(fs.Length - 1);
                        fs.Seek(-2, SeekOrigin.End);
                        continue;
                    case '}':
                        foundCurlyBracket = true;
                        break;
                    default:
                        fs.Seek(-2, SeekOrigin.Current);
                        continue;
                }
            }

            // If no curly bracket was found, the file is not a JSON list of objects
            if (!foundCurlyBracket)
            {
                return false;
            }

            // Add comma and new line
            fs.WriteByte((byte)',');
            fs.WriteByte((byte)'\n');

            // Serialize and write object
            var json = JsonConvert.SerializeObject(obj);
            json = "  " + json.Replace("\n", "\n  ");
            var bytes = Encoding.UTF8.GetBytes(json);
            fs.Write(bytes);

            // Close JSON list
            fs.WriteByte((byte)'\n');
            fs.WriteByte((byte)']');

            fs.Close();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
