using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientTests
    {
        private static int testCount;
        private World world;
        private MyClientWorld clientWorld;
        private EntityQuery connectingQuery;

        [SetUp]
        public void SetUp()
        {
            clientWorld = new MyClientWorld();
            world = clientWorld.World;
            //world.GetExistingSystem<ClientConnectSystem>().Enabled = false;
            connectingQuery = clientWorld.World.EntityManager.CreateEntityQuery(typeof(Connecting));
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
            world.GetExistingSystem<ClientConnectSystem>().Update();
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            /*
            UpdateWorld();
            UpdateWorld();
            */
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent)).CalculateEntityCount());
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

        [Test]
        public void ShouldHaveSystemsInCreatedWorld()
        {
            List<Type> systems = new List<Type>().AppendTypesWithAttribute<ClientSystemAttribute>();
            foreach (Type system in systems)
            {
                Assert.IsNotNull(world.GetExistingSystem(system), $"System: {system.Name}");
            }
        }


        [Test]
        public void WhenConnected_ShouldSetNetworkId()
        {
            world.CreateSingleton(new NetworkIdComponent { Value = 42 });
            world.EnqueueEvent(new ConnectionCompleteEvent());
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            world.GetHookSystem().Update();
            Assert.AreEqual(42, clientWorld.NetworkId);
        }

        [Test]
        public void WhenDisconnected_ShouldResetNetworkIdToZero()
        {
            WhenConnected_ShouldSetNetworkId();
            world.EnqueueEvent(new DisconnectedEvent());
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            world.GetHookSystem().Update();
            Assert.AreEqual(0, clientWorld.NetworkId);
        }

        private void UpdateWorld()
        {
            //clientWorld.World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            clientWorld.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }

        private static ClientOptions GetOptions()
        {
            return new ClientOptions
            {
                WorldName = $"TestClientWorld{testCount++}"
            };
        }

        private class MyClientWorld : ClientWorld
        {
            public CallbackName CallbackName;

            public new IWorldCreator WorldCreator => base.WorldCreator;

            public MyClientWorld() : base(GetOptions())
            {
                Connecting += () => CallbackName = CallbackName.Connecting;
                Connected += i => CallbackName = CallbackName.Connected;
                ConnectionFailed += s => CallbackName = CallbackName.ConnectionFailed;
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
    }
}