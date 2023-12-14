using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyGUI.Events;
using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Job;
using Application = System.Windows.Application;

namespace EasyGUI.Controls;

public partial class JobDisplay : INotifyPropertyChanged, IJobStatusSubscriber
{
    public static readonly DependencyProperty JobProperty = DependencyProperty.Register(
        nameof(Job),
        typeof(Job),
        typeof(JobDisplay),
        new PropertyMetadata(default(Job))
    );

    public static readonly DependencyProperty JobProgressTextProperty = DependencyProperty.Register(
        nameof(JobProgressText),
        typeof(string),
        typeof(JobDisplay),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty SelectedJobsProperty = DependencyProperty.Register(
        nameof(SelectedJobs),
        typeof(ObservableCollection<Job>),
        typeof(JobDisplay),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    private bool _isJobSelectedLocked;

    private bool _selectedJobsLocked;

    public JobDisplay()
    {
        InitializeComponent();
    }

    public Job Job
    {
        get => (Job)GetValue(JobProperty);
        set
        {
            SetValue(JobProperty, value);
            OnPropertyChanged();
            UpdateBreadcrumbs();
        }
    }

    public ObservableCollection<Job> SelectedJobs
    {
        get => (ObservableCollection<Job>)GetValue(SelectedJobsProperty);
        set
        {
            SetValue(SelectedJobsProperty, value);
            OnPropertyChanged();
        }
    }

    public string JobProgressText
    {
        get => (string)GetValue(JobProgressTextProperty);
        set
        {
            SetValue(JobProgressTextProperty, value);
            OnPropertyChanged();
        }
    }

    public string NameDisplay => $"#{Job.Id} - {Job.Name}";
    public string JobPaths => $"{Job.SourceFolder} \u2192 {Job.DestinationFolder}";

    public void OnJobStateChange(JobState state, Job job)
    {
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Job)));
    }

    public void OnJobProgress(Job job)
    {
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Job)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<JobEventArgs>? JobStarted;
    public event EventHandler<JobEventArgs>? JobEdited;
    public event EventHandler<JobEventArgs>? JobResumed;
    public event EventHandler<JobEventArgs>? JobDeleted;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == nameof(Job))
        {
            UpdateJobProgress();
            UpdateBreadcrumbs();
            UpdateButtons();
        }

        if (propertyName == nameof(SelectedJobs) && !_selectedJobsLocked)
        {
            _isJobSelectedLocked = true;
            JobCheckBox.IsChecked = SelectedJobs.Contains(Job);
            _isJobSelectedLocked = false;
        }
    }

    private void UpdateBreadcrumbs()
    {
        SetElementVisibility(FullBreadCrumb, Job.Type == JobType.Full);
        SetElementVisibility(DifferentialBreadCrumb, Job.Type == JobType.Differential);
        SetElementVisibility(IncrementalBreadCrumb, Job.Type == JobType.Incremental);
        SetElementVisibility(EndBreadCrumb, Job.State == JobState.End);
        SetElementVisibility(SourceBreadCrumb, Job.State == JobState.SourceScan);
        SetElementVisibility(DiffCalcBreadCrumb, Job.State == JobState.DifferenceCalculation);
        SetElementVisibility(StructureBreadCrumb, Job.State == JobState.DestinationStructureCreation);
        SetElementVisibility(CopyBreadCrumb, Job.State == JobState.Copy);
        SetElementVisibility(PausedBreadCrumb, Job.State != JobState.End && !Job.CurrentlyRunning);
    }

    private void UpdateButtons()
    {
        var paused = Job.State != JobState.End && !Job.CurrentlyRunning;
        SetElementVisibility(EditButton, Job.State == JobState.End);
        SetElementVisibility(StartButton, Job.State == JobState.End);
        SetElementVisibility(ResumeButton, paused);
        SetElementVisibility(DeleteButton, Job.State == JobState.End);
    }

    private void UpdateJobProgress()
    {
        if (Job.State == JobState.End)
        {
            JobProgressGrid.Visibility = Visibility.Collapsed;
            JobProgressText = "0 %";
            JobProgressBar.Value = 0;
            return;
        }

        JobProgressGrid.Visibility = Visibility.Visible;

        if (Job.State == JobState.Copy)
        {
            var progress = (float)Job.FilesCopied / Job.FilesCount;
            JobProgressText = $"{progress:P}";
            JobProgressBar.Value = progress * 100;
        }
    }

    private static void SetElementVisibility(UIElement element, bool visible)
    {
        element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        JobStarted?.Invoke(this, new JobEventArgs(Job));
    }

    private void JobDisplay_OnLoaded(object sender, RoutedEventArgs e)
    {
        Job.Subscribe(this);
        UpdateBreadcrumbs();
        UpdateJobProgress();
        UpdateButtons();
        SelectedJobs.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SelectedJobs));
    }

    private void JobCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (_isJobSelectedLocked)
            return;

        _selectedJobsLocked = true;
        SelectedJobs.Add(Job);
        _selectedJobsLocked = false;
    }

    private void JobCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (_isJobSelectedLocked)
            return;

        _selectedJobsLocked = true;
        SelectedJobs.Remove(Job);
        _selectedJobsLocked = false;
    }

    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        JobEdited?.Invoke(this, new JobEventArgs(Job));
    }

    private void ResumeButton_OnClick(object sender, RoutedEventArgs e)
    {
        JobResumed?.Invoke(this, new JobEventArgs(Job));
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        JobDeleted?.Invoke(this, new JobEventArgs(Job));
    }
}
