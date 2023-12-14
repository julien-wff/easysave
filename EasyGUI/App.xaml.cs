using System.Windows;
using System.Windows.Threading;
using EasyGUI.Resources;
using EasyLib.Files;
using MessageBox = System.Windows.MessageBox;

namespace EasyGUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Thread.CurrentThread.CurrentUICulture = ConfigManager.Instance.Language;
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        base.OnStartup(e);
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Console.Error.WriteLine(e.Exception);
        MessageBox.Show(
            string.Format(Strings.App_CrashPopup_Message, e.Exception.Message),
            Strings.App_CrashPopup_Title,
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
        Environment.Exit(1);
    }
}
