﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.CommandBufferHelpers;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode.Tests.WorldBaseTests
{
    public class Constructor : TestBase
    {
        private MyServerWorld myServerWorld;
        private MyClientWorld myClientWorld;

        [SetUp]
        public void SetUp_Constructor()
        {
            myServerWorld = new MyServerWorld();
            myClientWorld = new MyClientWorld();
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

        [Test]
        public void ShouldCreateWorldCreatedEventEntity()
        {
            myServerWorld.World.Update();
            //myServerWorld.World.GetExistingSystem<InitializationSystemGroup>().Update();
            Assert.AreEqual(1,
                myServerWorld.World.EntityManager.CreateEntityQuery(typeof(WorldCreated)).CalculateEntityCount());
        }

        public static IEnumerable<Type> WorldBaseSystemTypes =
            new List<Type>().AppendTypesWithAttribute<WorldBaseSystemAttribute>();
    }


    public class MyWorldBaseImpl<T> : WorldBase<T>
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
        public MyClientWorld() : base(new MyOptionsImpl(), ClientServerBootstrap.CreateClientWorld)
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

    [WorldBaseSystem]
    public class MySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {

        }
    }
}