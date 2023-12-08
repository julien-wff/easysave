using System.Windows;

namespace EasyGUI.Controls;

public partial class CreateButton
{
    public CreateButton()
    {
        InitializeComponent();
    }

    public event RoutedEventHandler? Click;

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }
}
