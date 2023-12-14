using System.Windows;
using EasyGUI.Resources;
using MessageBox = System.Windows.MessageBox;

namespace EasyGUI;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        using var mutex = new Mutex(false, "com.prosoft.EasyGUI");
        if (!mutex.WaitOne(1000, false))
        {
            MessageBox.Show(
                Strings.ResourceManager.GetString("MainWindow_InstanceError_Message")!,
                Strings.ResourceManager.GetString("MainWindow_InstanceError_Title")!,
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK
            );
            Environment.Exit(1);
            return;
        }

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
