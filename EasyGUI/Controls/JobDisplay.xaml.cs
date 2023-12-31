using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EasyGUI.Events;
using EasyGUI.Resources;
using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Job;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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

    public static readonly DependencyProperty IsRemoteProperty = DependencyProperty.Register(
        nameof(IsRemote),
        typeof(bool),
        typeof(JobDisplay),
        new PropertyMetadata(default(bool))
    );

    private readonly object _errorMessageLock = new();

    private string? _errorMessage;

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

    public bool IsRemote
    {
        get => (bool)GetValue(IsRemoteProperty);
        set
        {
            SetValue(IsRemoteProperty, value);
            OnPropertyChanged();
        }
    }

    public string NameDisplay => $"#{Job.Id} - {Job.Name}";

    public void OnJobStateChange(JobState state, Job job)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _errorMessage = null;
            OnPropertyChanged(nameof(Job));
        });
    }

    public void OnJobProgress(Job job)
    {
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Job)));
    }

    public void OnJobError(Exception error)
    {
        lock (_errorMessageLock)
        {
            if (_errorMessage != null)
                return;

            _errorMessage = error.Message;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            var jobName = Job.Name;

            Task.Run(() => MessageBox.Show(
                string.Format(Strings.JobDisplay_ErrorPopup_Message, jobName, error.Message),
                Strings.JobDisplay_ErrorPopup_Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            ));

            OnPropertyChanged(nameof(Job));
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<JobEventArgs>? JobStarted;
    public event EventHandler<JobEventArgs>? JobEdited;
    public event EventHandler<JobEventArgs>? JobResumed;
    public event EventHandler<JobEventArgs>? JobDeleted;
    public event EventHandler<JobEventArgs>? JobDiscarded;
    public event EventHandler<JobEventArgs>? JobPaused;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == nameof(Job))
        {
            UpdateJobProgress();
            UpdateBreadcrumbs();
            UpdateButtons();
            SetButtonsState(true);
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
        SetElementVisibility(ErrorBreadCrumb, !string.IsNullOrEmpty(_errorMessage));
    }

    private void UpdateButtons()
    {
        var paused = Job.State != JobState.End && !Job.CurrentlyRunning;
        SetElementVisibility(StartButton, Job.State == JobState.End);
        SetElementVisibility(EditButton, Job.State == JobState.End && !IsRemote);
        SetElementVisibility(DeleteButton, Job.State == JobState.End && !IsRemote);
        SetElementVisibility(ResumeButton, paused);
        SetElementVisibility(DiscardButton, paused);
        SetElementVisibility(PauseButton, Job.State != JobState.End && Job.CurrentlyRunning);
    }

    private void SetButtonsState(bool enabled)
    {
        EditButton.IsEnabled = enabled;
        StartButton.IsEnabled = enabled;
        DeleteButton.IsEnabled = enabled;
        ResumeButton.IsEnabled = enabled;
        DiscardButton.IsEnabled = enabled;
        PauseButton.IsEnabled = enabled;
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
            if (!float.IsNaN(progress))
            {
                JobProgressText = $"{progress:P}";
                JobProgressBar.Value = progress * 100;
            }
        }
    }

    private static void SetElementVisibility(UIElement element, bool visible)
    {
        element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        SetButtonsState(false);
        JobStarted?.Invoke(this, new JobEventArgs(Job));
    }

    private void JobDisplay_OnLoaded(object sender, RoutedEventArgs e)
    {
        Job.Subscribe(this);
        UpdateBreadcrumbs();
        UpdateJobProgress();
        UpdateButtons();
        SelectedJobs.CollectionChanged += _selectedJobs_CollectionChanged;
    }

    private void JobDisplay_OnUnloaded(object sender, RoutedEventArgs e)
    {
        Job.Unsubscribe(this);
        SelectedJobs.CollectionChanged -= _selectedJobs_CollectionChanged;
    }

    private void _selectedJobs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedJobs));
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
        SetButtonsState(false);
        JobResumed?.Invoke(this, new JobEventArgs(Job));
    }

    private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
    {
        SetButtonsState(false);
        JobDeleted?.Invoke(this, new JobEventArgs(Job));
    }

    private void DiscardButton_OnClick(object sender, RoutedEventArgs e)
    {
        SetButtonsState(false);
        JobDiscarded?.Invoke(this, new JobEventArgs(Job));
    }

    private void PauseButton_OnClick(object sender, RoutedEventArgs e)
    {
        SetButtonsState(false);
        JobPaused?.Invoke(this, new JobEventArgs(Job));
    }

    private void JobSource_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start("explorer.exe", Job.SourceFolder);
    }

    private void JobDestination_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Process.Start("explorer.exe", Job.DestinationFolder);
    }
}
