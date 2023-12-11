using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyGUI.Events;
using EasyLib.Enums;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobDisplay : INotifyPropertyChanged
{
    public static readonly DependencyProperty JobProperty = DependencyProperty.Register(
        nameof(Job),
        typeof(Job),
        typeof(JobDisplay),
        new PropertyMetadata(default(Job))
    );

    public static readonly DependencyProperty SelectedJobsProperty = DependencyProperty.Register(
        nameof(SelectedJobs),
        typeof(ObservableCollection<Job>),
        typeof(JobDisplay),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    public static readonly DependencyProperty JobSelectedProperty = DependencyProperty.Register(
        nameof(JobSelected),
        typeof(bool),
        typeof(JobDisplay),
        new PropertyMetadata(default(bool))
    );

    private bool _updatingCheckbox;

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
            _updatingCheckbox = true;
            SetValue(SelectedJobsProperty, value);
            OnPropertyChanged();
            _updatingCheckbox = false;
        }
    }

    public bool JobSelected
    {
        get => (bool)GetValue(JobSelectedProperty);
        set
        {
            _updatingCheckbox = true;
            SetValue(JobSelectedProperty, value);
            OnPropertyChanged();
            _updatingCheckbox = false;
        }
    }

    public string NameDisplay => $"#{Job.Id} - {Job.Name}";
    public string JobPaths => $"{Job.SourceFolder} \u2192 {Job.DestinationFolder}";

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<JobEventArgs> JobStarted;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == nameof(SelectedJobs) && !_updatingCheckbox)
        {
            JobSelected = SelectedJobs.Contains(Job);
        }

        if (propertyName == nameof(JobSelected) && !_updatingCheckbox)
        {
            if (JobSelected)
            {
                SelectedJobs.Add(Job);
            }
            else
            {
                SelectedJobs.Remove(Job);
            }
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
    }

    private static void SetBreadcrumbVisibility(UIElement breadCrumb, bool visible)
    {
        breadCrumb.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        JobStarted.Invoke(this, new JobEventArgs(Job));
    }

    private void JobDisplay_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateBreadcrumbs();
        SelectedJobs.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SelectedJobs));
    }
}
