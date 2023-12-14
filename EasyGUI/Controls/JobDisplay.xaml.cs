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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == nameof(Job))
        {
            UpdateJobProgress();
            UpdateBreadcrumbs();
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
        SetBreadcrumbVisibility(FullBreadCrumb, Job.Type == JobType.Full);
        SetBreadcrumbVisibility(DifferentialBreadCrumb, Job.Type == JobType.Differential);
        SetBreadcrumbVisibility(IncrementalBreadCrumb, Job.Type == JobType.Incremental);
        SetBreadcrumbVisibility(EndBreadCrumb, Job.State == JobState.End);
        SetBreadcrumbVisibility(SourceBreadCrumb, Job.State == JobState.SourceScan);
        SetBreadcrumbVisibility(DiffCalcBreadCrumb, Job.State == JobState.DifferenceCalculation);
        SetBreadcrumbVisibility(StructureBreadCrumb, Job.State == JobState.DestinationStructureCreation);
        SetBreadcrumbVisibility(CopyBreadCrumb, Job.State == JobState.Copy);
        SetBreadcrumbVisibility(PausedBreadCrumb, Job.State != JobState.End && !Job.CurrentlyRunning);
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

    private static void SetBreadcrumbVisibility(UIElement breadCrumb, bool visible)
    {
        breadCrumb.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
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
}
