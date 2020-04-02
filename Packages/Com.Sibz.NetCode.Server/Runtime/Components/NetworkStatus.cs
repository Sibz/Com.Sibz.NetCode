namespace Sibz.NetCode.Server
{
    public struct NetworkStatus : INetworkStatus<NetworkState>
    {
        public NetworkState NetworkState;
        public int InGameClientCount;
        public int ConnectionCount;
        public NetworkState State => NetworkState;
    }
}