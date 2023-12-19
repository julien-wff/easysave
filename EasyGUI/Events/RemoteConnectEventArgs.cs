using System.Net;

namespace EasyGUI.Events;

public class RemoteConnectEventArgs(EndPoint endPoint) : EventArgs
{
    public EndPoint EndPoint { get; } = endPoint;
}
