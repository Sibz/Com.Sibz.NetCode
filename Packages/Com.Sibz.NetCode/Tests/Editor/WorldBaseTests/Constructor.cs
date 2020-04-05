using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.EntityEvents;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode.Tests
{
    public class Constructor
    {
        private Func<World, string, World> creationMethod;
        private MyWorldBaseImpl current;
        private MyWorldManagerOptions worldManagerOptions;
        public World Create(World world, string str) => World.DefaultGameObjectInjectionWorld;

        [SetUp]
        public void SetUp() => worldManagerOptions = MyWorldManagerOptions.Defaults;

        [TearDown]
        public void TearDown_Constructor()
        {
            //current?.Dispose();
        }

        [Test]
        public void ShouldBindCallbacksToWorldManager()
        {
            var calledCount = 0;
            var wm = new MyWorldManager(worldManagerOptions);
            current = new MyWorldBaseImpl(wm);
            wm.CreateWorld(current.Systems);
            current.WorldCreated += () => calledCount++;
            current.WorldDestroyed += () => calledCount++;
            current.PreWorldDestroy += () => calledCount++;
            wm.InvokeAllCallbacks();
            Assert.AreEqual(3, calledCount);
        }

        [Test]
        public void ShouldHaveCorrectSystems()
        {
            var wm = new MyWorldManager(worldManagerOptions);
            current = new MyWorldBaseImpl(wm);
            wm.CreateWorld(current.Systems);
            Assert.IsNotNull(current.World.GetExistingSystem<EventComponentSystem>());
        }

        [Test]
        public void WhenWorldManagerIsNull_ShouldThrow() =>
            Assert.Catch<ArgumentNullException>(() => new MyWorldBaseImpl(null));

        [Test]
        public void ShouldSetSystems()
        {
            current = new MyWorldBaseImpl(new MyWorldManager(worldManagerOptions));
            Assert.IsNotNull(current.Systems);
        }

        [Test]
        public void ShouldAddMySystemToSystemsList()
        {
            current = new MyWorldBaseImpl(new MyWorldManager(worldManagerOptions));
            Assert.Contains(typeof(MySystem), current.Systems);
        }

        [Test]
        public void ShouldAddMySystem2ToSystemsList()
        {
            worldManagerOptions.Systems.Add(typeof(MySystem2));
            current = new MyWorldBaseImpl(new MyWorldManager(worldManagerOptions));
            Assert.Contains(typeof(MySystem2), current.Systems);
        }

        [Test]
        public void WhenOptionSpecified_ShouldCreateWorldFromConstructor()
        {
            worldManagerOptions.CreateWorldOnInstantiate = true;
            var worldManager = new MyWorldManager(worldManagerOptions);
            current = new MyWorldBaseImpl(worldManager);
            Assert.IsTrue(worldManager.CalledBootStrapCreateWorld);
        }

        [Test]
        public void WhenOptionNotSpecified_ShouldNotCreateWorldFromConstructor()
        {
            worldManagerOptions.CreateWorldOnInstantiate = false;
            var worldManager = new MyWorldManager(worldManagerOptions);
            current = new MyWorldBaseImpl(worldManager);
            Assert.IsFalse(worldManager.CalledBootStrapCreateWorld);
        }
    }


    public class MyWorldBaseImpl : WorldBase
    {
        //public BeginInitCommandBuffer Buffer => CommandBuffer;
        //public new IWorldOptionsBase Options => base.WorldManager.;
        public new List<Type> Systems => base.Systems;

        public MyWorldBaseImpl(IWorldManager worldManager) : base(worldManager)
        {
        }
    }

    /*public class MyClientWorld : MyWorldBaseImpl<ClientSimulationSystemGroup>
    {
        public MyClientWorld(string name = "TestClient") : base(new MyOptionsImpl
            {
                WorldName = name
            },
            ClientServerBootstrap.CreateClientWorld)
        {
        }
    }

    public class MyServerWorld : MyWorldBaseImpl<ServerSimulationSystemGroup>
    {
        public MyServerWorld() : base(new MyOptionsImpl(), ClientServerBootstrap.CreateServerWorld)
        {
        }
    }*/

    public class MyBaseOptionsImpl : INetworkEndpointSettings
    {
        public string WorldName { get; set; } = "Test";
        public List<GameObject> SharedDataPrefabs { get; } = new List<GameObject> { new GameObject("Test") };
        public List<Type> SystemImportAttributes { get; } = new List<Type>();
        public bool ConnectOnSpawn { get; set; }
        public int ConnectTimeout { get; set; } = 2;
        public string Address { get; set; } = "0.0.0.0";
        public ushort Port { get; set; } = 1000;
        public NetworkFamily NetworkFamily { get; set; } = NetworkFamily.Ipv4;
    }

    [ClientAndServerSystem]
    public class MySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
        }
    }

    public class MySystem2 : ComponentSystem
    {
        protected override void OnUpdate()
        {
        }
    }
}