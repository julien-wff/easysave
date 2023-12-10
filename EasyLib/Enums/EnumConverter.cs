namespace EasyLib.Enums;

public static class EnumConverter<T> where T : Enum
{
    private static readonly Dictionary<string, T> StringToEnumMap = new();
    private static readonly Dictionary<T, string> EnumToStringMap = new();

    static EnumConverter()
    {
        foreach (T enumValue in Enum.GetValues(typeof(T)))
        {
            var jobStateStr = enumValue.ToString();
            StringToEnumMap[jobStateStr] = enumValue;
            EnumToStringMap[enumValue] = jobStateStr;
        }
    }

    public static T ConvertToEnum(string enumValueStr)
    {
        enumValueStr = enumValueStr[0].ToString().ToUpper() + enumValueStr[1..];
        if (StringToEnumMap.TryGetValue(enumValueStr, out var enumValue))
        {
            return enumValue;
        }

        throw new ArgumentException($"Invalid job state string: {enumValueStr}", nameof(enumValueStr));
    }

    public static string ConvertToString(T enumValue)
    {
        if (EnumToStringMap.TryGetValue(enumValue, out var enumValueStr))
        {
            return enumValueStr;
        }

        throw new ArgumentException($"Invalid job state enum: {enumValue}", nameof(enumValue));
    }
}
