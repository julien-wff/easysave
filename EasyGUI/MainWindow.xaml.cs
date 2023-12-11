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
        CreateJobPopup.JobId = -1;
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
        var id = CreateJobPopup.JobId;
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

        var result = _jobManager.CheckJobRules(id, name, source, destination);

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
        var job = e.Job;
        var jobs = new List<Job> { job };
        RunJobs(jobs);
    }

    private void JobsHeader_OnStartButtonClick(object sender, RoutedEventArgs e)
    {
        var jobs = SelectedJobs.ToList();
        RunJobs(jobs);
    }

    private void RunJobs(IReadOnlyCollection<Job> jobs)
    {
        JobRunPopup.PopupTitle = $"Running {jobs.Count} job(s)";
        JobRunPopup.Visibility = Visibility.Visible;

        Dispatcher.InvokeAsync(async () =>
        {
            await Task.Delay(100);
            _jobManager.ExecuteJobs(jobs);
            JobRunPopup.Visibility = Visibility.Collapsed;
        });
    }

    private void JobsList_OnJobEdited(object? sender, JobEventArgs e)
    {
        var job = e.Job;
        CreateJobPopup.JobId = (int)job.Id;
        CreateJobPopup.PopupTitle = $"Edit job #{job.Id}";
        CreateJobPopup.JobName = job.Name;
        CreateJobPopup.JobSource = job.SourceFolder;
        CreateJobPopup.JobDestination = job.DestinationFolder;
        CreateJobPopup.JobType = job.Type;
        CreateJobPopup.ErrorMessage = null;
        CreateJobPopup.Visibility = Visibility.Visible;
    }
}
