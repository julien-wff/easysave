using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace EasyGUI.Controls;

public partial class RemoteConnectPopup : INotifyPropertyChanged
{
    private static readonly DependencyProperty HostProperty = DependencyProperty.Register(
        nameof(Host),
        typeof(string),
        typeof(RemoteConnectPopup),
        new PropertyMetadata(default(string))
    );

    public RemoteConnectPopup()
    {
        InitializeComponent();
    }

    public string Host
    {
        get => (string)GetValue(HostProperty);
        set
        {
            SetValue(HostProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? Connect;

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    private void ValidateButton_OnClick(object sender, RoutedEventArgs e)
    {
        Connect?.Invoke(this, e);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        HostPlaceHolder.Text = string.IsNullOrWhiteSpace(Host)
            ? "127.0.0.1:1234"
            : "";
    }
}
