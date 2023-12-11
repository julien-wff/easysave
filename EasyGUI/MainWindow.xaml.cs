using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EasyGUI.Events;
using EasyLib;
using EasyLib.Enums;
using EasyLib.Job;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace EasyGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly JobManager _jobManager;

    public MainWindow()
    {
        InitializeComponent();

        _jobManager = new JobManager();
        foreach (var job in _jobManager.GetJobs())
        {
            Jobs.Add(job);
        }
    }

    public ObservableCollection<Job> Jobs { get; } = new();
    public ObservableCollection<Job> SelectedJobs { get; } = new();

    private void JobsHeader_OnCreateButtonClick(object sender, RoutedEventArgs e)
    {
        CreateJobPopup.PopupTitle = "Create a new job";
        CreateJobPopup.JobName = "";
        CreateJobPopup.JobSource = "";
        CreateJobPopup.JobDestination = "";
        CreateJobPopup.JobType = null;
        CreateJobPopup.ErrorMessage = null;
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

    private void CreateJobPopup_OnValidateJob(object sender, RoutedEventArgs e)
    {
        var name = CreateJobPopup.JobName;
        var source = CreateJobPopup.JobSource;
        var destination = CreateJobPopup.JobDestination;
        var type = CreateJobPopup.JobType;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(source) ||
            string.IsNullOrWhiteSpace(destination) || type == null)
        {
            CreateJobPopup.ErrorMessage = "All fields are required";
            return;
        }

        var result = _jobManager.CheckJobRules(-1, name, source, destination);

        if (result != JobCheckRule.Valid)
        {
            CreateJobPopup.ErrorMessage = EnumConverter<JobCheckRule>.ConvertToString(result);
            return;
        }

        var job = _jobManager.CreateJob(name, source, destination, type.Value);
        JobsList.Jobs.Add(job);
        CreateJobPopup.Visibility = Visibility.Collapsed;
    }

    private void JobsList_OnJobStarted(object? sender, JobEventArgs e)
    {
        JobRunPopup.PopupTitle = "Running 1 job";
        JobRunPopup.Visibility = Visibility.Visible;
    }
}
