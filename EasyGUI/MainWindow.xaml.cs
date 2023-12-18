using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using EasyGUI.Events;
using EasyGUI.Resources;
using EasyLib.Enums;
using EasyLib.Job;
using EasyLib.JobManager;
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

        _jobManager = new LocalJobManager();
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
        CreateJobPopup.PopupTitle = Strings.ResourceManager.GetString("CreateJobPopup_PopupTitle_CreateJob")!;
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

        if (e.Key == Key.Escape && SettingsPopup.Visibility == Visibility.Visible)
        {
            SettingsPopup.Visibility = Visibility.Collapsed;
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
            CreateJobPopup.ErrorMessage =
                Strings.ResourceManager.GetString("CreateJobPopup_PopupTitle_AllFieldsRequired")!;
            return;
        }

        var result = _jobManager.CheckJobRules(id, name, source, destination);

        if (result != JobCheckRule.Valid)
        {
            CreateJobPopup.ErrorMessage = EnumConverter<JobCheckRule>.ConvertToString(result);
            return;
        }

        if (id == -1)
        {
            var job = _jobManager.CreateJob(name, source, destination, type.Value);
            Jobs.Add(job);
        }
        else
        {
            var job = Jobs.First(j => j.Id == id);
            _jobManager.EditJob(job, name, source, destination, type.Value);
            UpdateJob(job);
        }

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
        Dispatcher.Invoke(() => _jobManager.ExecuteJobs(jobs));
    }

    private void JobsList_OnJobEdited(object? sender, JobEventArgs e)
    {
        var titleString = Strings.ResourceManager.GetString("CreateJobPopup_PopupTitle_EditingJob")!;
        var job = e.Job;
        CreateJobPopup.JobId = (int)job.Id;
        CreateJobPopup.PopupTitle = string.Format(titleString, job.Id);
        CreateJobPopup.JobName = job.Name;
        CreateJobPopup.JobSource = job.SourceFolder;
        CreateJobPopup.JobDestination = job.DestinationFolder;
        CreateJobPopup.JobType = job.Type;
        CreateJobPopup.ErrorMessage = null;
        CreateJobPopup.Visibility = Visibility.Visible;
    }

    private void JobsHeader_OnSettingsButtonClick(object sender, RoutedEventArgs e)
    {
        SettingsPopup.Visibility = Visibility.Visible;
    }

    private void JobsList_OnJobResumed(object? sender, JobEventArgs e)
    {
        var job = e.Job;
        Dispatcher.Invoke(() => _jobManager.ResumeJob(job));
    }

    private void JobsList_OnJobDeleted(object? sender, JobEventArgs e)
    {
        var job = e.Job;
        Dispatcher.Invoke(() => _jobManager.DeleteJob(job));
        Jobs.Remove(job);
    }

    private void JobsList_OnJobDiscarded(object? sender, JobEventArgs e)
    {
        var job = e.Job;
        Dispatcher.Invoke(() =>
        {
            _jobManager.CancelJob(job);
            UpdateJob(job);
        });
    }

    private void JobsList_OnJobPaused(object? sender, JobEventArgs e)
    {
        var job = e.Job;
        Dispatcher.Invoke(() => _jobManager.PauseJob(job));
    }

    private void UpdateJob(Job job)
    {
        var jobIndex = Jobs.IndexOf(job);
        Jobs.Remove(job);
        Jobs.Insert(jobIndex, job);
    }

    private void SettingsPopup_OnReloadConfig(object? sender, EventArgs e)
    {
        _jobManager.ReloadConfig();
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        // destroy job manager
        _jobManager.CleanStop();
    }
}
