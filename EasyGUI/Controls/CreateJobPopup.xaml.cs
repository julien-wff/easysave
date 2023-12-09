using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyGUI.Controls.Buttons;

namespace EasyGUI.Controls;

public partial class CreateJobPopup : INotifyPropertyChanged
{
    public static readonly DependencyProperty PopupTitleProperty = DependencyProperty.Register(
        nameof(PopupTitle),
        typeof(string),
        typeof(CreateButton),
        new PropertyMetadata("Title")
    );

    public static readonly DependencyProperty JobNameProperty = DependencyProperty.Register(
        nameof(JobName),
        typeof(string),
        typeof(CreateButton),
        new PropertyMetadata("Job name")
    );

    public CreateJobPopup()
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

    public string JobName
    {
        get => (string)GetValue(JobNameProperty);
        set
        {
            SetValue(JobNameProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void FocusFirstField()
    {
        JobNameTextBox.Focus();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }
}
