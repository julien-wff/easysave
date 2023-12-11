using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EasyGUI.Controls;

public partial class FolderSelector : INotifyPropertyChanged
{
    public static readonly DependencyProperty FolderPathProperty = DependencyProperty.Register(
        nameof(FolderPath),
        typeof(string),
        typeof(FolderSelector),
        new PropertyMetadata(default(string))
    );

    public FolderSelector()
    {
        InitializeComponent();
    }

    public string FolderPath
    {
        get => (string)GetValue(FolderPathProperty);
        set
        {
            SetValue(FolderPathProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            FolderPath = dialog.SelectedPath;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
