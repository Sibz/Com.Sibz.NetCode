using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Sibz.CommandBufferHelpers;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
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
        public void WhenOptionsIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => current = new MyWorldBaseImpl(null, Create, new List<Type>()));
        }

        [Test]
        public void WhenCreationMethodIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() =>
                current = new MyWorldBaseImpl(new MyOptionsImpl(), null, new List<Type>()));
        }

        [Test]
        public void WhenSystemsIsNull_ShouldNotThrow()
        {
            current = new MyWorldBaseImpl(new MyOptionsImpl(), Create, new List<Type>());
        }

        [Test]
        public void ShouldSetSystems()
        {
            current = new MyWorldBaseImpl(new MyOptionsImpl(), Create, null);
            Assert.IsNotNull(current.Systems);
        }

        [Test]
        public void ShouldAddMySystemToSystemsList()
        {
            current = new MyWorldBaseImpl(new MyOptionsImpl(), Create, null);
            Assert.Contains(typeof(MySystem), current.Systems);
        }

        [Test]
        public void ShouldAddMySystem2ToSystemsList()
        {
            current = new MyWorldBaseImpl(new MyOptionsImpl(), Create, new List<Type> {typeof(MySystem2)});
            Assert.Contains(typeof(MySystem2), current.Systems);
        }

        [Test]
        public void ShouldSetOptions()
        {
            current = new MyWorldBaseImpl(new MyOptionsImpl(), Create, null);
            Assert.IsNotNull(current.Options);
        }

        [Test]
        public void WhenOptionSpecified_ShouldCreateWorldFromConstructor()
        {
            MyWorldManager worldManager = new MyWorldManager();
            current = new MyWorldBaseImpl(new MyOptionsImpl() {ConnectOnSpawn = true}, Create, null, worldManager);
            Assert.IsTrue(worldManager.CalledCreateWorld);
        }

        [Test]
        public void WhenOpttionNotSpecified_ShouldNotCreateWorldFromConstructor()
        {
            MyWorldManager worldManager = new MyWorldManager();
            current = new MyWorldBaseImpl(new MyOptionsImpl() {ConnectOnSpawn = false}, Create, null, worldManager);
            Assert.IsFalse(worldManager.CalledCreateWorld);
        }

        public static IEnumerable<Type> WorldBaseSystemTypes =
            new List<Type>().AppendTypesWithAttribute<ClientAndServerSystemAttribute>();
    }

    public class MyWorldManager : IWorldManager
    {
        public bool CalledCreateWorld;

        public void CreateWorld()
        {
            CalledCreateWorld = true;
        }

        public void DestroyWorld()
        {
        }
    }

    public class MyWorldBaseImpl : WorldBase<ClientSimulationSystemGroup>
    {
        public BeginInitCommandBuffer Buffer => CommandBuffer;
        public new IWorldOptionsBase Options => base.Options;
        public new List<Type> Systems => base.Systems;

        public MyWorldBaseImpl(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems, IWorldManager worldManager = null) : base(options, creationMethod, systems,
            worldManager)
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