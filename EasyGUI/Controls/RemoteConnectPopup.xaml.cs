using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyGUI.Events;
using EasyLib.Files;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Strings = EasyGUI.Resources.Strings;

namespace EasyGUI.Controls;

public partial class RemoteConnectPopup : INotifyPropertyChanged
{
    private static readonly DependencyProperty HostProperty = DependencyProperty.Register(
        nameof(Host),
        typeof(string),
        typeof(RemoteConnectPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
        nameof(ErrorMessage),
        typeof(string),
        typeof(RemoteConnectPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty HostIpProperty = DependencyProperty.Register(
        nameof(HostIp),
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

    public string? ErrorMessage
    {
        get => (string?)GetValue(ErrorMessageProperty);
        set
        {
            SetValue(ErrorMessageProperty, value);
            OnPropertyChanged();
        }
    }

    public string? HostIp
    {
        get => (string?)GetValue(HostIpProperty);
        set
        {
            SetValue(HostIpProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler<RemoteConnectEventArgs>? Connect;

    private static string? GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return null;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    private void ValidateButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            ErrorMessage = Strings.RemoteConnectPopup_Error_HostNorSpecified;
            return;
        }

        var operands = Host.Split(':');

        switch (operands.Length)
        {
            case 1:
                ErrorMessage = Strings.RemoteConnectPopup_Error_PortNorSpecified;
                return;
            case > 2:
                ErrorMessage = Strings.RemoteConnectPopup_Error_InvalidAdress;
                return;
        }

        if (!int.TryParse(operands[1], out var port))
        {
            ErrorMessage = Strings.RemoteConnectPopup_Error_InvalidPort;
            return;
        }

        if (port is < 0 or > 65535)
        {
            ErrorMessage = Strings.RemoteConnectPopup_Error_InvalidPort;
            return;
        }

        if (!IPAddress.TryParse(operands[0], out var ip))
        {
            ErrorMessage = Strings.RemoteConnectPopup_Error_InvalidAdress;
            return;
        }

        // Ping the host to check if it's reachable
        var ping = new Ping();
        var reply = ping.Send(ip, 1000);

        if (reply.Status != IPStatus.Success)
        {
            ErrorMessage = Strings.RemoteConnectPopup_Error_HostUnreachable;
            return;
        }

        ErrorMessage = null;

        var endpoint = new IPEndPoint(ip, port);
        var eventArgs = new RemoteConnectEventArgs(endpoint);

        Connect?.Invoke(this, eventArgs);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName == nameof(ErrorMessage))
        {
            ErrorMessageTextBlock.Visibility = string.IsNullOrWhiteSpace(ErrorMessage)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        if (propertyName == nameof(HostIp))
        {
            ServerIpTextBlock.Visibility = string.IsNullOrWhiteSpace(HostIp)
                ? Visibility.Collapsed
                : Visibility.Visible;

            ServerIpTextBlock.Text = string.Format(
                Strings.RemoteConnectPopup_HostAddress,
                HostIp
            );
        }
    }

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        HostPlaceHolder.Text = string.IsNullOrWhiteSpace(Host)
            ? "127.0.0.1:1234"
            : "";
    }

    private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ValidateButton_OnClick(sender, e);
        }
    }

    private void RemoteConnectPopup_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ConfigManager.Instance.ServerPort is not null)
        {
            HostIp = $"{GetLocalIpAddress()}:{ConfigManager.Instance.ServerPort}";
        }
    }
}
