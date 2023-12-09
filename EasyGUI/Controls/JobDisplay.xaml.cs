using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using EasyLib.Events;
using EasyLib.Job;

namespace EasyGUI.Controls;

public partial class JobDisplay : INotifyPropertyChanged, IJobStatusSubscriber
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
        };
    }

    public Job? Job => DataContext as Job;

    public string NameDisplay
    {
        get
        {
            if (Job != null)
            {
                return $"#{Job.Id} - {Job.Name}";
            }

            return string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
