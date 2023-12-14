using System.Windows;
using EasyLib.Files;

namespace EasyGUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = ConfigManager.Instance.Language;
        base.OnStartup(e);
    }
}
