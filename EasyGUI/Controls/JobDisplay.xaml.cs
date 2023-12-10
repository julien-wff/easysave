using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using EasyGUI.Events;
using EasyLib.Enums;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobDisplay : INotifyPropertyChanged
{
    public JobDisplay()
    {
        InitializeComponent();

        DataContextChanged += (_, _) =>
        {
            var bindingExpression = BindingOperations.GetBindingExpression(this, DataContextProperty);
            bindingExpression?.UpdateTarget();
            OnPropertyChanged(nameof(Job));
            OnPropertyChanged(nameof(NameDisplay));
            UpdateBreadcrumbs();
        };
    }

    public Job? Job => DataContext as Job;

    public string NameDisplay => Job != null ? $"#{Job.Id} - {Job.Name}" : string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<JobEventArgs> JobStarted;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UpdateBreadcrumbs()
    {
        SetBreadcrumbVisibility(FullBreadCrumb, Job?.Type == JobType.Full);
        SetBreadcrumbVisibility(DifferentialBreadCrumb, Job?.Type == JobType.Differential);
        SetBreadcrumbVisibility(IncrementalBreadCrumb, Job?.Type == JobType.Incremental);
        SetBreadcrumbVisibility(EndBreadCrumb, Job?.State == JobState.End);
        SetBreadcrumbVisibility(SourceBreadCrumb, Job?.State == JobState.SourceScan);
        SetBreadcrumbVisibility(DiffCalcBreadCrumb, Job?.State == JobState.DifferenceCalculation);
        SetBreadcrumbVisibility(StructureBreadCrumb, Job?.State == JobState.DestinationStructureCreation);
        SetBreadcrumbVisibility(CopyBreadCrumb, Job?.State == JobState.Copy);
    }

    private static void SetBreadcrumbVisibility(UIElement breadCrumb, bool visible)
    {
        breadCrumb.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Job == null)
            return;

        JobStarted.Invoke(this, new JobEventArgs(Job));
    }
}
