using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode.Tests.WorldBaseTests
{
    public class Constructor : TestBase
    {
        private Func<World, string, World> creationMethod;

        private MyWorldBaseImpl current;

        public World Create(World world, string str)
        {
            return World.DefaultGameObjectInjectionWorld;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp_Constructor()
        {
            Debug.Log("OneTimeSetUp_Constructor");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown_Constructor()
        {
            //  myClientWorld.Dispose();
        }

        [TearDown]
        public void TearDown_Constructor()
        {
            current?.Dispose();
        }

        [Test]
        public void WhenWorldManagerIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => new MyWorldBaseImpl(null));
        }

        [Test]
        public void ShouldSetSystems()
        {
            current = new MyWorldBaseImpl(new MyWorldManager());
            Assert.IsNotNull(current.Systems);
        }

        [Test]
        public void ShouldAddMySystemToSystemsList()
        {
            current = new MyWorldBaseImpl(new MyWorldManager());
            Assert.Contains(typeof(MySystem), current.Systems);
        }

        [Test]
        public void ShouldAddMySystem2ToSystemsList()
        {
            current = new MyWorldBaseImpl(new MyWorldManager());
            Assert.Contains(typeof(MySystem2), current.Systems);
        }

        [Test]
        public void WhenOptionSpecified_ShouldCreateWorldFromConstructor()
        {
            MyWorldManager worldManager = new MyWorldManager(true);
            current = new MyWorldBaseImpl(worldManager);
            Assert.IsTrue(worldManager.CalledCreateWorld);
        }

        [Test]
        public void WhenOptionNotSpecified_ShouldNotCreateWorldFromConstructor()
        {
            MyWorldManager worldManager = new MyWorldManager();
            current = new MyWorldBaseImpl(worldManager);
            Assert.IsFalse(worldManager.CalledCreateWorld);
        }
    }

    public class MyWorldManager : IWorldManager
    {
        public bool CalledCreateWorld;

        public void CreateWorld()
        {
            CalledCreateWorld = true;
        }

        public MyWorldManager(bool create = false)
        {
            CreateWorldOnInstantiate = create;
        }
        public World World { get; private set; }
        public bool CreateWorldOnInstantiate { get; }
        public List<Type> GetSystemsList()
        {
            return new List<Type> {typeof(MySystem2)};
        }

        public void CreateWorld(List<Type> systems)
        {
            CalledCreateWorld = true;
        }

        public void DestroyWorld()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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

    public class MyOptionsImpl : IWorldOptionsBase
    {
        public string WorldName { get; set; } = "Test";
        public List<GameObject> SharedDataPrefabs { get; } = new List<GameObject> {new GameObject("Test")};
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