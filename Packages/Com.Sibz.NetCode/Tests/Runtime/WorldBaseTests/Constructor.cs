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



        public static IEnumerable<Type> WorldBaseSystemTypes =
            new List<Type>().AppendTypesWithAttribute<ClientAndServerSystemAttribute>();
    }

    public class MyWorldBaseImpl<T> : WorldBase<T>
        where T : ComponentSystemGroup
    {
        public BeginInitCommandBuffer Buffer => CommandBuffer;
        public new IWorldOptionsBase Options => base.Options;
        public new List<Type> Systems => base.Systems;

        public MyWorldBaseImpl(IWorldOptionsBase options, Func<World, string, World> creationMethod,
            List<Type> systems = null) : base(options, creationMethod, systems)
        {
        }
    }

    public class MyClientWorld : MyWorldBaseImpl<ClientSimulationSystemGroup>
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