namespace Sibz.NetCode.Client
{
    public enum NetworkState : byte
    {
        Uninitialised,
        InitialRequest,
        ConnectingToServer,
        GoingInGame,
        Connected,
        Disconnected
    }
}