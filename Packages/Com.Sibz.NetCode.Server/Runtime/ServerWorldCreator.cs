using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public class ServerWorldCreator : WorldCreatorBase
    {
        public override Type InitSystemGroup => typeof(ServerInitializationSystemGroup);
        public override Type SimSystemGroup => typeof(ServerSimulationSystemGroup);
        public override Type PresSystemGroup => null;

        public ServerWorldCreator(IWorldCreatorOptions options) :
            base(options)
        {
        }

        protected override World BootStrapCreateWorld(string worldName)
        {
            return ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, worldName);
        }
    }
}