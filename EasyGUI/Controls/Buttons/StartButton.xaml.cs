using System.Windows;

namespace EasyGUI.Controls.Buttons;

public partial class StartButton
{
    public StartButton()
    {
        InitializeComponent();
    }

    public event RoutedEventHandler? Click;

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }
}
