using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Server
{
    public class ServerWorldCreator : WorldCreatorBase
    {
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