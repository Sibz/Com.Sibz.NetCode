using System;
using System.Collections.Generic;
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
        private MyClientWorld myClientWorld;

        [OneTimeSetUp]
        public void OneTimeSetUp_Constructor()
        {
            Debug.Log("OneTimeSetUp_Constructor");

            myClientWorld = new MyClientWorld();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown_Constructor()
        {
            myClientWorld.Dispose();
        }

        [Test]
        public void ShouldCreateServerWorld()
        {
            using (MyServerWorld myServerWorld = new MyServerWorld())
            {
                Assert.IsNotNull(myServerWorld.World);
                Assert.IsTrue(World.All.Contains(myServerWorld.World));
                Assert.IsNotNull(myServerWorld.World.GetExistingSystem<ServerSimulationSystemGroup>());
            }
        }

        [Test]
        public void ShouldCreateClientWorld()
        {
            Assert.IsNotNull(myClientWorld.World);
            Assert.IsTrue(World.All.Contains(myClientWorld.World));
            Assert.IsNotNull(myClientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>());
        }

        [Test]
        public void ShouldAddEventComponentSystem()
        {
            Assert.IsNotNull(myClientWorld.World.GetExistingSystem<EventComponentSystem>());
        }

        [Test]
        public void ShouldCreateBuffer()
        {
            Assert.IsNotNull(myClientWorld.Buffer);
        }

        [Test]
        public void ShouldCreateBufferThatIsCreated()
        {
            Assert.IsTrue(myClientWorld.Buffer.Buffer.IsCreated);
        }

        [Test]
        public void ShouldBeAbleToUseBuffer()
        {
            myClientWorld.Buffer.Buffer.CreateEntity();
        }

        [Test]
        public void ShouldImportWorldBaseSystems(
            [ValueSource(nameof(WorldBaseSystemTypes))]
            Type systemType)
        {
            Assert.IsNotNull(myClientWorld.World.GetExistingSystem(systemType));
        }

        [Test]
        public void ShouldImportSharedDataPrefabs()
        {
            Assert.IsNotNull(GameObject.Find("Test"));
        }

        [Test]
        public void ShouldCreateWorldCreatedEventEntity()
        {
            MyClientWorld test = new MyClientWorld("ClientTestTEST");
            test.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
            test.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            World.Update();
            EntityQuery q = test.World.EntityManager.CreateEntityQuery(typeof(WorldCreated));
            Assert.AreEqual(1, q.CalculateEntityCount());
            test.Dispose();
        }

        public static IEnumerable<Type> WorldBaseSystemTypes =
            new List<Type>().AppendTypesWithAttribute<ClientAndServerSystemAttribute>();
    }

    public class MyWorldBaseImpl<T> : WorldBase<T, NetworkStatus>
        where T : ComponentSystemGroup
    {
        public BeginInitCommandBuffer Buffer => CommandBuffer;


        public MyWorldBaseImpl(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null) : base(options, creationMethod, systems)
        {
        }
    }

    public class MyClientWorld : MyWorldBaseImpl<ClientSimulationSystemGroup>
    {
        public MyClientWorld(string name = "TestClient") : base(new MyOptionsImpl { WorldName = name }, ClientServerBootstrap.CreateClientWorld)
        {
        }
    }

    public class MyServerWorld : MyWorldBaseImpl<ServerSimulationSystemGroup>
    {
        public MyServerWorld() : base(new MyOptionsImpl(), ClientServerBootstrap.CreateServerWorld)
        {
        }
    }

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
}