using System.Resources;
using EasyLib.Files;

namespace EasyCLI.Localization;

public static class Loc
{
    private static readonly ResourceManager ResourceManager = GetResourceManager();

    private static ResourceManager GetResourceManager()
    {
        Thread.CurrentThread.CurrentUICulture = ConfigManager.Instance.Language;
        return Strings.Strings.ResourceManager;
    }

    public static string T(string key, params object[] args)
    {
        var translation = ResourceManager.GetString(key) ?? key;
        return string.Format(translation, args);
    }
}
