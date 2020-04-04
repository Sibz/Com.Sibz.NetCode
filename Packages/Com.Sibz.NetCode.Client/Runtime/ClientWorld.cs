using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase
    {
        protected ClientOptions Options { get; }


        private void Connect()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
        }

        public ClientWorld(IWorldManager worldManager) : base(worldManager)
        {
        }
    }
}