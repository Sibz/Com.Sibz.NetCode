namespace Sibz.NetCode.Client
{
    public struct NetworkStatus : INetworkStatus<NetworkState>
    {
        public NetworkState NetworkState;
        public NetworkState State => NetworkState;
    }
}