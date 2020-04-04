using System;
using System.Collections.Generic;
using Sibz.WorldSystemHelpers;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests
{
    public class MyWorldManager : WorldManagerBase
    {
        public bool CalledBootStrapCreateWorld;
        public bool CalledImportPrefabs;

        protected override World BootStrapCreateWorld(string name)
        {
            CalledBootStrapCreateWorld = true;
            return ClientServerBootstrap.CreateClientWorld(NetCodeFixture.DefaultWorld, name);
        }

        protected override void InjectSystems(List<Type> systems) =>
            World.ImportSystemsFromList<ClientSimulationSystemGroup>(systems);

        protected override void ImportPrefabs() => CalledImportPrefabs = true;

        public MyWorldManager(IWorldManagerOptions options, IWorldCallbackProvider callbackProvider = null) : base(options, callbackProvider)
        {
        }

        public void InvokeAllCallbacks()
        {
            CallbackProvider?.WorldCreated?.Invoke();
            CallbackProvider?.WorldDestroyed?.Invoke();
            CallbackProvider?.PreWorldDestroy?.Invoke();
        }
    }
}