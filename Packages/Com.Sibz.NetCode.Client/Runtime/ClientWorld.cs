using Unity.Entities;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase
    {
        protected ClientOptions Options { get; }

        public ClientWorld(IWorldManager worldManager) : base(worldManager)
        {
        }


        private void Connect()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
        }
    }
}