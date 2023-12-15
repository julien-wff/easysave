using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyLib.Files;
using Application = System.Windows.Forms.Application;

namespace EasyGUI.Controls;

public partial class SettingsPopup : INotifyPropertyChanged
{
    private static readonly DependencyProperty EncryptedFileTypesProperty = DependencyProperty.Register(
        nameof(EncryptedFileTypes),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty PriorityExtensionsProperty = DependencyProperty.Register(
        nameof(PriorityExtensions),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty XorKeyProperty = DependencyProperty.Register(
        nameof(XorKey),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(CultureInfo))
    );

    private static readonly DependencyProperty EasyCryptoPathProperty = DependencyProperty.Register(
        nameof(EasyCryptoPath),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty CompanySoftwareProcessProperty = DependencyProperty.Register(
        nameof(CompanySoftwareProcess),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(string))
    );

    private static readonly DependencyProperty MaxFileSizeProperty = DependencyProperty.Register(
        nameof(MaxFileSize),
        typeof(string),
        typeof(SettingsPopup),
        new PropertyMetadata(default(string))
    );

    private string? _baseCulture;

    public SettingsPopup()
    {
        InitializeComponent();
    }

    public string EncryptedFileTypes
    {
        get => (string)GetValue(EncryptedFileTypesProperty);
        set
        {
            SetValue(EncryptedFileTypesProperty, value);
            OnPropertyChanged();
        }
    }

    public string PriorityExtensions
    {
        get => (string)GetValue(PriorityExtensionsProperty);
        set
        {
            SetValue(PriorityExtensionsProperty, value);
            OnPropertyChanged();
        }
    }

    public string XorKey
    {
        get => (string)GetValue(XorKeyProperty);
        set
        {
            SetValue(XorKeyProperty, value);
            OnPropertyChanged();
        }
    }

    public string EasyCryptoPath
    {
        get => (string)GetValue(EasyCryptoPathProperty);
        set
        {
            SetValue(EasyCryptoPathProperty, value);
            OnPropertyChanged();
        }
    }

    public string CompanySoftwareProcess
    {
        get => (string)GetValue(CompanySoftwareProcessProperty);
        set
        {
            SetValue(CompanySoftwareProcessProperty, value);
            OnPropertyChanged();
        }
    }

    public string MaxFileSize
    {
        get => (string)GetValue(MaxFileSizeProperty);
        set
        {
            SetValue(MaxFileSizeProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    private void UpdateProperties()
    {
        LanguageComboBox.SelectedIndex = ConfigManager.Instance.Language.Name switch
        {
            "en-US" => 0,
            "fr-FR" => 1,
            _ => 0
        };

        EncryptedFileTypes = string.Join(
            ", ",
            ConfigManager.Instance.EncryptedFileExtensions.Select(ext => ext[1..])
        );

        PriorityExtensions = string.Join(
            ", ",
            ConfigManager.Instance.PriorityFileExtensions.Select(ext => ext[1..])
        );

        XorKey = ConfigManager.Instance.XorKey;

        LogTypeComboBox.SelectedIndex = ConfigManager.Instance.LogFormat switch
        {
            ".json" => 0,
            ".xml" => 1,
            _ => 0
        };

        EasyCryptoPath = ConfigManager.Instance.EasyCryptoPath ?? "";
        CompanySoftwareProcess = ConfigManager.Instance.CompanySoftwareProcessPath ?? "";
        MaxFileSize = ConfigManager.Instance.MaxFileSize.ToString();
    }

    private void ValidateButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Update language
        var culture = (LanguageComboBox.SelectedIndex switch
        {
            0 => new CultureInfo("en-US"),
            1 => new CultureInfo("fr-FR"),
            _ => CultureInfo.DefaultThreadCurrentCulture
        })!;
        ConfigManager.Instance.Language = culture;

        // Update encrypted file types
        ConfigManager.Instance.EncryptedFileExtensions = EncryptedFileTypes.Split(",")
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(ext => "." + ext)
            .ToList();

        // Update priority extensions
        ConfigManager.Instance.PriorityFileExtensions = PriorityExtensions.Split(",")
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(ext => "." + ext)
            .ToList();

        // Update xor key
        ConfigManager.Instance.XorKey = XorKey;

        // Update log format
        ConfigManager.Instance.LogFormat = LogTypeComboBox.SelectedIndex switch
        {
            0 => ".json",
            1 => ".xml",
            _ => ".json"
        };

        // Update EasyCrypto path
        ConfigManager.Instance.EasyCryptoPath = string.IsNullOrWhiteSpace(EasyCryptoPath) ? null : EasyCryptoPath;

        // Update company software process
        ConfigManager.Instance.CompanySoftwareProcessPath = string.IsNullOrWhiteSpace(CompanySoftwareProcess)
            ? null
            : CompanySoftwareProcess;

        // Update max file size
        if (ulong.TryParse(MaxFileSize, out var maxFileSize))
        {
            ConfigManager.Instance.MaxFileSize = maxFileSize;
        }

        // Save config
        ConfigManager.Instance.WriteConfig();

        // Hide popup
        Visibility = Visibility.Collapsed;

        // Reload window if the language changed
        if (_baseCulture != culture.Name)
        {
            Application.Restart();
            Process.GetCurrentProcess().Kill();
        }

        // Invoke reload config event
        ReloadConfig?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? ReloadConfig;

    private void SettingsPopup_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateProperties();
        _baseCulture = ConfigManager.Instance.Language.Name;
    }
}
