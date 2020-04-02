using System;
using System.Collections.Generic;
using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode
{
    public class ClientWorld : WorldBase<ClientSimulationSystemGroup, NetworkStatus>
    {
        protected ClientOptions Options { get; }

        protected ClientWorld(ClientOptions options, List<Type> systems = null)
            : base(options ?? new ClientOptions(), ClientServerBootstrap.CreateClientWorld,
                systems.AppendTypesWithAttribute<ClientSystemAttribute>())
        {
            Options = options ?? new ClientOptions();

            if (Options.ConnectOnSpawn)
            {
                Connect();
            }
        }

        private void Connect()
        {
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(Options.Address, Options.Port, Options.NetworkFamily);
        }
    }
}