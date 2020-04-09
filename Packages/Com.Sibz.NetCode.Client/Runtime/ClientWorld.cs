using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase
    {
        protected ClientOptions Options { get; }

        public ClientWorld(ClientOptions options) : base(options, new ClientWorldCreator(options))
        {
        }


        private void Connect()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
        }
    }
}