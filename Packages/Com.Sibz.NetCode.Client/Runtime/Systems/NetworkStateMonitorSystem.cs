namespace Sibz.NetCode.Client
{
    public class NetworkStateMonitorSystem : NetworkStateMonitorSystemBase<NetworkStatus, NetworkState, UpdateStateJob>
    {
        protected override UpdateStateJob CreateJob()
        {
            return new UpdateStateJob();
        }
    }
}