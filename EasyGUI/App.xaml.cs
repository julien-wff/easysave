using System.Diagnostics;
using System.Windows;

namespace EasyGUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            Trace.WriteLine("Application Starting.");
            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"The application terminated unexpectedly during startup. Exception: {ex}");
            throw;
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            Trace.WriteLine("Application Ended Successfully.");
            base.OnExit(e);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"The application terminated unexpectedly during exit. Exception: {ex}");
            throw;
        }
        finally
        {
            Trace.Close();
        }
    }
}
