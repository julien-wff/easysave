using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EasyGUI.Controls.Buttons;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace EasyGUI.Controls;

public partial class BreadCrumb : INotifyPropertyChanged
{
    public static readonly DependencyProperty BreadCrumbTextProperty = DependencyProperty.Register(
        nameof(BreadCrumbText),
        typeof(string),
        typeof(CreateButton),
        new PropertyMetadata("BreadCrumb"));

    public static readonly DependencyProperty BreadCrumbColorProperty = DependencyProperty.Register(
        nameof(BreadCrumbColor),
        typeof(Brush),
        typeof(CreateButton),
        new PropertyMetadata(Brushes.Red)
    );

    public BreadCrumb()
    {
        InitializeComponent();
    }

    public string BreadCrumbText
    {
        get => (string)GetValue(BreadCrumbTextProperty);
        set
        {
            SetValue(BreadCrumbTextProperty, value);
            OnPropertyChanged();
        }
    }

    public Brush BreadCrumbColor
    {
        get => (Brush)GetValue(BreadCrumbColorProperty);
        set
        {
            SetValue(BreadCrumbColorProperty, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
