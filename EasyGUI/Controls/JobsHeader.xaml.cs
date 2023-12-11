using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobsHeader : INotifyPropertyChanged
{
    private static readonly RoutedEvent CreateButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(CreateButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(JobsHeader)
    );

    private static readonly DependencyProperty JobsProperty = DependencyProperty.Register(
        nameof(Jobs),
        typeof(ObservableCollection<Job>),
        typeof(JobsHeader),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    private static readonly DependencyProperty SelectedJobsProperty = DependencyProperty.Register(
        nameof(SelectedJobs),
        typeof(ObservableCollection<Job>),
        typeof(JobsHeader),
        new PropertyMetadata(default(ObservableCollection<Job>))
    );

    public JobsHeader()
    {
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

    public event RoutedEventHandler CreateButtonClick
    {
        add => AddHandler(CreateButtonClickEvent, value);
        remove => RemoveHandler(CreateButtonClickEvent, value);
    }

    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CreateButtonClickEvent));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        SelectedJobs.Clear();
        foreach (var job in Jobs)
        {
            SelectedJobs.Add(job);
        }
    }

    private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        SelectedJobs.Clear();
    }

    private void JobsHeader_OnLoaded(object sender, RoutedEventArgs e)
    {
        SelectedJobs.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SelectedJobs));
    }
}
