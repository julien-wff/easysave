using System.Collections.ObjectModel;
using EasyGUI.Events;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobsList
{
    public JobsList()
    {
        DataContext = this;
        InitializeComponent();
    }

    public ObservableCollection<Job> Jobs { get; } = new();

    public event EventHandler<JobEventArgs> JobStarted;

    private void JobDisplay_OnJobStarted(object? sender, JobEventArgs e)
    {
        JobStarted.Invoke(this, e);
    }
}
