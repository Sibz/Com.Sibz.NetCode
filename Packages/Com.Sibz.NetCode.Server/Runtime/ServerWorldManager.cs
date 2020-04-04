using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public class ServerWorldManager : WorldManagerBase
    {
        public ServerWorldManager(IWorldManagerOptions options) : base(options)
        {
        }

        protected override World BootStrapCreateWorld(string worldName) =>
            ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, worldName);

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ServerSimulationSystemGroup>(systems);
    }
}