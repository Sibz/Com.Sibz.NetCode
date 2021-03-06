﻿using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Client
{
    public class ClientWorldCreator : WorldCreatorBase
    {
        private const string WorldNotCreatedError =
            "{0}: Unable to connect, world is not created.";

        public ClientWorldCreator(IWorldCreatorOptions options) : base(
            options)
        {
        }

        protected override World BootStrapCreateWorld(string worldName)
        {
            return ClientServerBootstrap.CreateClientWorld(
                World.DefaultGameObjectInjectionWorld,
                worldName);
        }
    }
}