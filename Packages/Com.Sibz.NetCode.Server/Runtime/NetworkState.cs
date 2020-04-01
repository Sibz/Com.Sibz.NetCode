namespace Sibz.NetCode
{
    public enum NetworkState : byte
    {
        Uninitialised,
        ListenRequested,
        ConnectionLive,
        Disconnected
    }
}