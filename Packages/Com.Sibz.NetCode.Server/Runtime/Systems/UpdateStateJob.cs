namespace Sibz.NetCode.Server
{
    public struct UpdateStateJob : INetworkStateChangeJob<NetworkStatus>
    {
        public bool Listening;
        public int ConnectionCount;
        public int InGameCount;

        public void Execute(ref NetworkStatus status)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (status.State == NetworkState.Uninitialised)
            {
                return;
            }

            if (status.State == NetworkState.Listening && !Listening)
            {
                status.NetworkState = NetworkState.Disconnected;
            }

            if (status.State != NetworkState.Listening)
            {
                status.ConnectionCount = 0;
                status.InGameClientCount = 0;
                return;
            }

            status.ConnectionCount = ConnectionCount;
            status.InGameClientCount = InGameCount;
        }
    }
}