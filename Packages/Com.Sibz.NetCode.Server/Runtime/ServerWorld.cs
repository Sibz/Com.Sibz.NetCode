using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Server
{
    public class ServerWorld : WorldBase<ServerSimulationSystemGroup>
    {
        protected ServerOptions Options { get; }
        protected Entity NetworkStatusEntity;
        protected readonly NetworkStreamReceiveSystem NetworkStreamReceiveSystem;

        public ServerWorld(ServerOptions options = null)
            : base(options??new ServerOptions(), ClientServerBootstrap.CreateServerWorld)
        {
            Options = options??new ServerOptions();

            NetworkStatusEntity =
                World.EntityManager.CreateEntity(typeof(NetworkStatus));

            NetworkStreamReceiveSystem = World.GetExistingSystem<NetworkStreamReceiveSystem>();

            if (Options.ConnectOnSpawn)
            {
                Listen();
            }
        }

        public void Listen()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
            NetworkStatus networkStatus = new NetworkStatus
            {
                State = NetworkStreamReceiveSystem.Listen(endPoint)
                    ? NetworkState.ListenRequested
                    : NetworkState.ListenFailed
            };
            World.EntityManager.SetComponentData(NetworkStatusEntity, networkStatus);
        }
    }
}