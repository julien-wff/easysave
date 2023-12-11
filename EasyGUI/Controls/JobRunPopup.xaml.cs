using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EasyGUI.Controls;

public partial class JobRunPopup : INotifyPropertyChanged
{
    public static readonly DependencyProperty PopupTitleProperty = DependencyProperty.Register(
        nameof(PopupTitle),
        typeof(string),
        typeof(JobRunPopup),
        new PropertyMetadata(default(string))
    );

    public JobRunPopup()
    {
        InitializeComponent();
    }

    public string PopupTitle
    {
        get => (string)GetValue(PopupTitleProperty);
        set
        {
            SetValue(PopupTitleProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
