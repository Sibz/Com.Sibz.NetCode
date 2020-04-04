namespace Sibz.NetCode.Server
{
    public enum NetworkState : byte
    {
        Uninitialised,
        Listening,
        Disconnected,
        ListenFailed
    }
}