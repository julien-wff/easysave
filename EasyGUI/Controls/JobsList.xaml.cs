using System.Collections.ObjectModel;
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
}
