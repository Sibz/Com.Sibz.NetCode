using System;
using NUnit.Framework;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientTests
    {
        private World world;
        private MyClientWorld clientWorld;
        private static int testCount;
        private EntityQuery connectingQuery;

        [SetUp]
        public void SetUp()
        {
            clientWorld = new MyClientWorld();
            world = clientWorld.World;
            connectingQuery = clientWorld.World.EntityManager.CreateEntityQuery(typeof(Connecting));
        }

        [TearDown]
        public void TearDown()
        {
            clientWorld.Dispose();
        }

        [Test]
        public void Disconnect_ShouldCreateSingleton()
        {
            clientWorld.Disconnect();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(Disconnect)).CalculateEntityCount());
        }

        [Test]
        public void Connect_WhenWorldInSotCreated_ShouldThrow()
        {
            clientWorld.WorldCreator.Dispose();
            Assert.Catch<InvalidOperationException>(() => clientWorld.Connect());
        }

        [Test]
        public void Connect_ShouldCreateConnectingEntity()
        {
            clientWorld.Connect();
            Assert.AreEqual(1, connectingQuery.CalculateEntityCount());
        }

        [Test]
        public void Connect_WhenCreatingConnectingEntity_ShouldSetStateToInitiating()
        {
            clientWorld.Connect();
            Assert.AreEqual(NetworkState.InitialRequest, connectingQuery.GetSingleton<Connecting>().State);
        }

        [Test]
        public void Connect_ShouldCreateConnectionInitiatedEvent()
        {
            clientWorld.Connect();
            UpdateWorld();
            UpdateWorld();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenConnectingEvent_ShouldCallback()
        {
            world.CreateSingleton<ConnectionInitiatedEvent>();
            world.GetHookSystem().Update();
            Assert.AreEqual(CallbackName.Connecting, clientWorld.CallbackName);
        }

        [Test]
        public void WhenConnectedEvent_ShouldCallback()
        {
            world.CreateSingleton<ConnectionCompleteEvent>();
            world.GetHookSystem().Update();
            Assert.AreEqual(CallbackName.Connected, clientWorld.CallbackName);
        }

        [Test]
        public void WhenConnectFailedEvent_ShouldCallback()
        {
            world.CreateSingleton<ConnectionFailedEvent>();
            world.GetHookSystem().Update();
            Assert.AreEqual(CallbackName.ConnectionFailed, clientWorld.CallbackName);
        }

        [Test]
        public void WhenDisconnectedEvent_ShouldCallback()
        {
            world.CreateSingleton<DisconnectedEvent>();
            world.GetHookSystem().Update();
            Assert.AreEqual(CallbackName.Disconnected, clientWorld.CallbackName);
        }

        private void UpdateWorld()
        {
            clientWorld.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }

        private class MyClientWorld : ClientWorld
        {
            public CallbackName CallbackName;

            public new IWorldCreator WorldCreator => base.WorldCreator;
            public MyClientWorld() : base(GetOptions())
            {
                Connecting += () => CallbackName = CallbackName.Connecting;
                Connected += (i) => CallbackName = CallbackName.Connected;
                ConnectionFailed += (s) => CallbackName = CallbackName.ConnectionFailed;
                Disconnected += () => CallbackName = CallbackName.Disconnected;
            }
        }

        private enum CallbackName
        {
            None,
            Connecting,
            Connected,
            ConnectionFailed,
            Disconnected
        }

        private static ClientOptions GetOptions()
        {
            return new ClientOptions
            {
                WorldName = $"TestClientWorld{testCount++}"
            };
        }
    }
}