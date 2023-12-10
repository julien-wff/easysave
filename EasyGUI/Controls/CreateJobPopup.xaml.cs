using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyLib.Enums;

namespace EasyGUI.Controls;

public partial class CreateJobPopup : INotifyPropertyChanged
{
    public delegate void ValidateJobHandler(object sender, RoutedEventArgs e);

    public static readonly DependencyProperty PopupTitleProperty = DependencyProperty.Register(
        nameof(PopupTitle),
        typeof(string),
        typeof(CreateJobPopup),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty JobNameProperty = DependencyProperty.Register(
        nameof(JobName),
        typeof(string),
        typeof(CreateJobPopup),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty JobSourceProperty = DependencyProperty.Register(
        nameof(JobSource),
        typeof(string),
        typeof(CreateJobPopup),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty JobDestinationProperty = DependencyProperty.Register(
        nameof(JobDestination),
        typeof(string),
        typeof(CreateJobPopup),
        new PropertyMetadata(default(string))
    );

    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
        nameof(ErrorMessage),
        typeof(string),
        typeof(CreateJobPopup),
        new PropertyMetadata(default(string))
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

    public string JobSource
    {
        get => (string)GetValue(JobSourceProperty);
        set
        {
            SetValue(JobSourceProperty, value);
            OnPropertyChanged();
        }
    }

    public string JobDestination
    {
        get => (string)GetValue(JobDestinationProperty);
        set
        {
            SetValue(JobDestinationProperty, value);
            OnPropertyChanged();
        }
    }

    public JobType? JobType
    {
        get => JobTypeComboBox.SelectedIndex switch
        {
            0 => EasyLib.Enums.JobType.Full,
            1 => EasyLib.Enums.JobType.Differential,
            2 => EasyLib.Enums.JobType.Incremental,
            _ => null
        };
        set
        {
            JobTypeComboBox.SelectedIndex = value switch
            {
                EasyLib.Enums.JobType.Full => 0,
                EasyLib.Enums.JobType.Differential => 1,
                EasyLib.Enums.JobType.Incremental => 2,
                _ => -1
            };
            OnPropertyChanged();
        }
    }

    public string? ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set
        {
            SetValue(ErrorMessageProperty, value);
            ErrorMessageTextBlock.Visibility = value is not null ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event ValidateJobHandler? ValidateJob;

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

    private void ValidateButton_OnClick(object sender, RoutedEventArgs e)
    {
        ValidateJob?.Invoke(this, e);
    }
}
