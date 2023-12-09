using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace EasyGUI.Controls;

public partial class BaseButton
{
    public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(
        nameof(ButtonText),
        typeof(string),
        typeof(CreateButton),
        new PropertyMetadata("Button"));

    public static readonly DependencyProperty ButtonColorProperty = DependencyProperty.Register(
        nameof(ButtonColor),
        typeof(Brush),
        typeof(CreateButton),
        new PropertyMetadata(Brushes.Red)
    );

    public static readonly DependencyProperty ButtonIconProperty = DependencyProperty.Register(
        nameof(ButtonIcon),
        typeof(ImageSource),
        typeof(CreateButton)
    );

    public BaseButton()
    {
        InitializeComponent();
    }

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public Brush ButtonColor
    {
        get => (Brush)GetValue(ButtonColorProperty);
        set => SetValue(ButtonColorProperty, value);
    }

    public ImageSource ButtonIcon
    {
        get => (ImageSource)GetValue(ButtonIconProperty);
        set => SetValue(ButtonIconProperty, value);
    }

    public event RoutedEventHandler? Click;

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }
}
