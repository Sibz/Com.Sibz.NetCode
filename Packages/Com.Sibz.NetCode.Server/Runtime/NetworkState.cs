namespace Sibz.NetCode.Server
{
    public enum NetworkState : byte
    {
        Uninitialised,
        ListenRequested,
        Listening,
        Disconnected,
        ListenFailed
    }
}