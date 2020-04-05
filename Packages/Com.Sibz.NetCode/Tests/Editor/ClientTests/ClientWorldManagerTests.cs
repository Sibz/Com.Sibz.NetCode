using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientWorldManagerTests
    {
        private ClientWorldManager worldManager;
        private ClientOptions options;
        private int testCount;
        private EntityQuery connectingQuery;
        private EntityQuery connectingEventQuery;

        private readonly List<Type> systems =
            new List<Type>()
                .AppendTypesWithAttribute<ClientAndServerSystemAttribute>()
                .AppendTypesWithAttribute<ClientSystemAttribute>();

        [SetUp]
        public void SetUp()
        {
            options = new ClientOptions() { WorldName = $"TestClient{testCount++}" };
            worldManager = new ClientWorldManager(options);
            worldManager.CreateWorld(systems);
            connectingQuery = worldManager.World.EntityManager.CreateEntityQuery(typeof(Connecting));
            connectingEventQuery = worldManager.World.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent));
        }

        [Test]
        public void Connect_WhenSettingIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => worldManager.Connect(null));
        }

        [Test]
        public void Connect_WhenWorldInSotCreated_ShouldThrow()
        {
            worldManager.DestroyWorld();
            Assert.Catch<InvalidOperationException>(() => worldManager.Connect(options));
        }

        [Test]
        public void Connect_ShouldCreateConnectingEntity()
        {
            worldManager.Connect(options);
            Assert.AreEqual(1, connectingQuery.CalculateEntityCount());
        }

        [Test]
        public void Connect_ShouldCreateConnectionInitiatedEvent()
        {
            worldManager.Connect(options);
            UpdateWorld();
            Assert.AreEqual(1, connectingEventQuery.CalculateEntityCount());
        }

        [Test]
        public void Connect_ShouldCallback()
        {
            bool success = false;

            void OnConnecting()
            {
                success = true;
            }

            worldManager.CallbackProvider = new MyClientCallbackProvider { Connecting = OnConnecting };
            worldManager.Connect(options);
            Assert.IsTrue(success);
        }

        private void UpdateWorld()
        {
            worldManager.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            worldManager.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }

        private class MyClientCallbackProvider : IClientWorldCallbackProvider
        {
            public Action Connecting { get; set; }
            public Action Connected { get; set; }
            public Action ConnectionFailed { get; set; }
            public Action Disconnected { get; set; }
        }
    }
}