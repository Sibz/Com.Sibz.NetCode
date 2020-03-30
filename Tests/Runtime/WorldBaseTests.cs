using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Sibz.CommandBufferHelpers;
using Sibz.NetCode.Tests.Util;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sibz.NetCode.Tests
{
    [TestFixture]
    public class WorldBaseTests
    {
        public class Constructor : TestBase
        {
            private MyWorldBaseImpl myServerWorld;
            private MyWorldBaseImpl myClientWorld;

            [SetUp]
            public void SetUp_Constructor()
            {
                myServerWorld = new MyWorldBaseImpl(new MyOptionsImpl(), false);
                myClientWorld = new MyWorldBaseImpl(new MyOptionsImpl(), true);
            }

            [TearDown]
            public void TearDown_Constructor()
            {
                myServerWorld.Dispose();
                myClientWorld.Dispose();
            }

            [Test]
            public void ShouldCreateServerWorld()
            {
                Assert.IsNotNull(myServerWorld.World);
                Assert.IsTrue(World.All.Contains(myServerWorld.World));
                Assert.IsNotNull(myServerWorld.World.GetExistingSystem<ServerSimulationSystemGroup>());
            }

            [Test]
            public void ShouldCreateClientWorld()
            {
                Assert.IsNotNull(myClientWorld.World);
                Assert.IsTrue(World.All.Contains(myClientWorld.World));
                Assert.IsNotNull(myClientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>());
            }

            [Test]
            public void ShouldCreateBuffer()
            {
                Assert.IsNotNull(myServerWorld.Buffer);
            }

            [Test]
            public void ShouldCreateBufferThatIsCreated()
            {
                Assert.IsTrue(myServerWorld.Buffer.Buffer.IsCreated);
            }

            [Test]
            public void ShouldBeAbleToUseBuffer()
            {
                myServerWorld.Buffer.Buffer.CreateEntity();
            }

            [Test]
            public void ShouldImportWorldBaseSystems([ValueSource(nameof(WorldBaseSystemTypes))]
                Type systemType)
            {
                Assert.IsNotNull(myServerWorld.World.GetExistingSystem(systemType));
            }

            [Test]
            public void ShouldImportSharedDataPrefabs()
            {
                Assert.IsNotNull(GameObject.Find("Test"));
            }

            public static IEnumerable<Type> WorldBaseSystemTypes = Assembly
                .GetAssembly(typeof(WorldBaseSystemAttribute)).GetTypes()
                .Where(x => !(x.GetCustomAttribute<WorldBaseSystemAttribute>() is null));
        }

        public class ConstructorBootstrapped : TestBase
        {
            private MyWorldBaseImpl myServerWorld;

            [SetUp]
            public void SetUp_Constructor()
            {
                myServerWorld = new MyWorldBaseImpl(new MyOptionsImpl(), false);
            }

            [TearDown]
            public void TearDown_Constructor()
            {
                myServerWorld.Dispose();
            }

            [Test]
            public void ShouldCreateWorldCreatedEventEntity()
            {
                myServerWorld.World.Update();
                //myServerWorld.World.GetExistingSystem<InitializationSystemGroup>().Update();
                Assert.AreEqual(1,
                    myServerWorld.World.EntityManager.CreateEntityQuery(typeof(WorldCreated)).CalculateEntityCount());
            }
        }
    }

    public class MyWorldBaseImpl : WorldBase
    {
        public BeginInitCommandBuffer Buffer => CommandBuffer;

        public MyWorldBaseImpl(IWorldOptionsBase options, bool isClient) : base(options, isClient)
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
}