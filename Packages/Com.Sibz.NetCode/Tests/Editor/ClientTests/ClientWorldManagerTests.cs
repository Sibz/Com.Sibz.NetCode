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
        private ClientWorldCreator worldCreator;
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
            worldCreator = new ClientWorldCreator(options);
            worldCreator.CreateWorld();
            connectingQuery = worldCreator.World.EntityManager.CreateEntityQuery(typeof(Connecting));
            connectingEventQuery = worldCreator.World.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent));
        }

        [TearDown]
        public void TearDown() => worldCreator.Dispose();

        [Test]
        public void Connect_WhenSettingIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => worldCreator.Connect(null));
        }

        [Test]
        public void Connect_WhenWorldInSotCreated_ShouldThrow()
        {
            worldCreator.Dispose();
            Assert.Catch<InvalidOperationException>(() => worldCreator.Connect(options));
        }

        [Test]
        public void Connect_ShouldCreateConnectingEntity()
        {
            worldCreator.Connect(options);
            Assert.AreEqual(1, connectingQuery.CalculateEntityCount());
        }

        [Test]
        public void Connect_WhenCreatingConnectingEntity_ShouldSetStateToInitiating()
        {
            worldCreator.Connect(options);
            Assert.AreEqual(NetworkState.InitialRequest, connectingQuery.GetSingleton<Connecting>().State);
        }

        [Test]
        public void Connect_ShouldCreateConnectionInitiatedEvent()
        {
            worldCreator.Connect(options);
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

            worldCreator.CallbackProvider = new MyClientCallbackProvider { Connecting = OnConnecting };
            worldCreator.Connect(options);
            Assert.IsTrue(success);
        }

        private void UpdateWorld()
        {
            worldCreator.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            worldCreator.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
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