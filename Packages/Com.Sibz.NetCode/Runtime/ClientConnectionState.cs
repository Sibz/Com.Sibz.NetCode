namespace Sibz.NetCode
{
    public enum ClientConnectionState : ushort
    {
        InitialRequest,
        ConnectingToServer,
        GoingInGame
    }
}