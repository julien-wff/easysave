using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyGUI.Events;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobsList : INotifyPropertyChanged
{
    public static readonly DependencyProperty JobsProperty = DependencyProperty.Register(
        nameof(Jobs),
        typeof(ObservableCollection<Job>),
        typeof(JobsList),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    public static readonly DependencyProperty SelectedJobsProperty = DependencyProperty.Register(
        nameof(SelectedJobs),
        typeof(ObservableCollection<Job>),
        typeof(JobsList),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    public JobsList()
    {
        DataContext = this;
        InitializeComponent();
    }

    public ObservableCollection<Job> Jobs
    {
        get => (ObservableCollection<Job>)GetValue(JobsProperty);
        set
        {
            SetValue(JobsProperty, value);
            OnPropertyChanged();
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

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<JobEventArgs>? JobStarted;

    public event EventHandler<JobEventArgs>? JobEdited;

    public event EventHandler<JobEventArgs>? JobResumed;

    private void JobDisplay_OnJobStarted(object? sender, JobEventArgs e)
    {
        JobStarted?.Invoke(this, e);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void JobDisplay_OnJobEdited(object? sender, JobEventArgs e)
    {
        JobEdited?.Invoke(this, e);
    }

    private void JobDisplay_OnJobResumed(object? sender, JobEventArgs e)
    {
        JobResumed?.Invoke(this, e);
    }
}
