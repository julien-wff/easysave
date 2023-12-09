using System.Windows;

namespace EasyGUI.Controls;

public partial class JobsHeader
{
    private static readonly RoutedEvent CreateButtonClickEvent = EventManager.RegisterRoutedEvent(
        nameof(CreateButtonClick),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(JobsHeader)
    );

    public JobsHeader()
    {
        InitializeComponent();
    }

    public event RoutedEventHandler CreateButtonClick
    {
        add => AddHandler(CreateButtonClickEvent, value);
        remove => RemoveHandler(CreateButtonClickEvent, value);
    }

    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CreateButtonClickEvent));
    }
}
