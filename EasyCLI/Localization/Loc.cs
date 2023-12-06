using System.Globalization;
using System.Resources;

namespace EasyCLI.Localization;

public static class Loc
{
    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string AppLangFile = Path.Combine(AppDataPath, "EasySave", "LANG");
    private static readonly ResourceManager ResourceManager = GetResourceManager();

    private static ResourceManager GetResourceManager()
    {
        if (File.Exists(AppLangFile))
        {
            var lang = File.ReadAllText(AppLangFile);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
        }
        else
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
        }

        return Strings.Strings.ResourceManager;
    }

    public static string T(string key, params object[] args)
    {
        var translation = ResourceManager.GetString(key) ?? key;
        return string.Format(translation, args);
    }
}
