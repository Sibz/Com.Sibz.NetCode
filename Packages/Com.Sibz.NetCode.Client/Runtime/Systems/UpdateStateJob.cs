namespace Sibz.NetCode.Client
{
    public struct UpdateStateJob : INetworkStateChangeJob<NetworkStatus>
    {
        public void Execute(ref NetworkStatus statusComponent)
        {
            throw new System.NotImplementedException();
        }
    }
}