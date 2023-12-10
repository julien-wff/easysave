using System.Windows;
using System.Windows.Input;
using EasyLib;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace EasyGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var jm = new JobManager();
        foreach (var job in jm.GetJobs())
        {
            JobsList.Jobs.Add(job);
        }
    }

    private void JobsHeader_OnCreateButtonClick(object sender, RoutedEventArgs e)
    {
        CreateJobPopup.PopupTitle = "Create a new job";
        CreateJobPopup.JobName = "";
        CreateJobPopup.JobSource = "";
        CreateJobPopup.JobDestination = "";
        CreateJobPopup.JobType = null;
        CreateJobPopup.Visibility = Visibility.Visible;
        CreateJobPopup.FocusFirstField();
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && CreateJobPopup.Visibility == Visibility.Visible)
        {
            CreateJobPopup.Visibility = Visibility.Collapsed;
        }
    }
}
