using System;
using NUnit.Framework;
using Sibz.NetCode.Client;
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
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent)).CalculateEntityCount());
        }

        private void UpdateWorld()
        {
            clientWorld.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }

        private class MyClientWorld : ClientWorld
        {
            public new IWorldCreator WorldCreator => base.WorldCreator;
            public MyClientWorld() : base(GetOptions())
            {
            }
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