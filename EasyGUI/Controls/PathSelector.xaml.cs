using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EasyGUI.Controls;

public enum PathSelectorType
{
    File,
    Folder
}

public partial class PathSelector : INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(
        nameof(SelectedPath),
        typeof(string),
        typeof(PathSelector),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty PathTypeProperty = DependencyProperty.Register(
        nameof(PathSelectorType),
        typeof(PathSelectorType),
        typeof(PathSelector),
        new PropertyMetadata(default(PathSelectorType))
    );

    public static readonly DependencyProperty InitialDirectoryProperty = DependencyProperty.Register(
        nameof(InitialDirectory),
        typeof(string),
        typeof(PathSelector),
        new PropertyMetadata(default(string))
    );

    public PathSelector()
    {
        InitializeComponent();
    }

    public string SelectedPath
    {
        get => (string)GetValue(SelectedPathProperty);
        set
        {
            SetValue(SelectedPathProperty, value);
            OnPropertyChanged();
        }
    }

    public PathSelectorType PathType
    {
        get => (PathSelectorType)GetValue(PathTypeProperty);
        set
        {
            SetValue(PathTypeProperty, value);
            OnPropertyChanged();
        }
    }

    public string? InitialDirectory
    {
        get => (string?)GetValue(InitialDirectoryProperty);
        set => SetValue(InitialDirectoryProperty, value);
    }

    public string? Filter { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (PathType == PathSelectorType.File)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = Filter;
            dialog.InitialDirectory = InitialDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = dialog.FileName;
            }
        }
        else
        {
            var dialog = new FolderBrowserDialog();
            dialog.InitialDirectory =
                InitialDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = dialog.SelectedPath;
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
