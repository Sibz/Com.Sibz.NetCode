using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.Tests.Util;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.NetCode.Tests
{
    [TestFixture]
    public class WorldBaseTests
    {
        public class Constructor :  TestBase
        {
            private MyWorldBaseImpl myWorldBase;

            [SetUp]
            public void SetUp_Constructor()
            {
                myWorldBase = new MyWorldBaseImpl(new MyOptionsImpl(), true);
            }

            [Test]
            public void ShouldCreateWorld()
            {
                Assert.IsNotNull(myWorldBase.World);
                Assert.IsTrue(World.All.Contains(myWorldBase.World));
            }
        }
    }

    public class MyWorldBaseImpl : WorldBase
    {
        public EntityCommandBuffer Buffer => CommandBuffer.Buffer;
        public MyWorldBaseImpl(IWorldOptionsBase options, bool isClient) : base(options, isClient)
        {

        }
    }

    public class MyOptionsImpl : IWorldOptionsBase
    {
        public string WorldName { get; set; } = "Test";
        public List<GameObject> SharedDataPrefabs { get; } = new List<GameObject>();
        public List<Type> SystemImportAttributes { get; } = new List<Type>();
        public bool ConnectOnSpawn { get; set; }
        public int ConnectTimeout { get; set; } = 2;
        public string Address { get; set; } = "0.0.0.0";
        public ushort Port { get; set; } = 1000;
        public NetworkFamily NetworkFamily { get; set; } = NetworkFamily.Ipv4;
    }
}