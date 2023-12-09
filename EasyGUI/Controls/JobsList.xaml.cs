using System.Collections.ObjectModel;
using EasyLib;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobsList
{
    public JobsList()
    {
        DataContext = this;

        var jm = new JobManager();
        foreach (var job in jm.GetJobs())
        {
            Jobs.Add(job);
        }

        InitializeComponent();
    }

    public ObservableCollection<Job> Jobs { get; set; } = new();
}
