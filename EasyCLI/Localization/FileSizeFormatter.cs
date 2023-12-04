namespace EasyCLI.Localization;

public static class FileSizeFormatter
{
    public static string Format(ulong bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        var order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }

        return $"{bytes:0.#} {sizes[order]}";
    }
}
