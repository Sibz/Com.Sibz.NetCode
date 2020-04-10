using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase, IClientWorldCallbackProvider
    {
        public Action Connecting { get; set; }
        public Action Connected { get; set; }
        public Action ConnectionFailed { get; set; }
        public Action Disconnected { get; set; }
        protected new ClientOptions Options { get; }

        public ClientWorld(ClientOptions options) : base(options, new ClientWorldCreator(options))
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }


        private void Connect()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
        }
    }
}