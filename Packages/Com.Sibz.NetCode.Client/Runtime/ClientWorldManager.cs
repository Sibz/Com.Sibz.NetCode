using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Client
{
    public class ClientWorldManager : WorldManagerBase, IClientWorldManager
    {
        public void Connect(INetworkEndpointSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var endPoint = NetworkEndPoint.Parse(settings.Address, settings.Port, settings.NetworkFamily);
            /*World.GetNetworkStreamReceiveSystem()
                .Connect();*/
        }

        public ClientWorldManager(IWorldManagerOptions options, IWorldCallbackProvider callbackProvider = null) : base(options, callbackProvider)
        {
        }

        protected override World BootStrapCreateWorld(string worldName) => throw new NotImplementedException();

        protected override void InjectSystems(List<Type> systems) => throw new NotImplementedException();
    }
}