using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectClientSystemTests
    {
        private World world;
        private MyDisconnectClientSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"TestDisconnectClientSystem{testCount++}");
            world.CreateSystem<BeginInitializationEntityCommandBufferSystem>();
            world.CreateSystem<EventComponentSystem>();
            world.CreateSystem<EndInitializationEntityCommandBufferSystem>();
            system = world.CreateSystem<MyDisconnectClientSystem>();
        }

        [Test]
        public void WhenDisconnectClientEntityDoesNotExist_ShouldNotRun()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenDisconnectClientEntityExist_ShouldRun()
        {
            world.CreateSingleton<DisconnectClient>();
            system.Update();
            Assert.IsTrue(system.DidUpdate);
        }

        [Test]
        public void ShouldAddDisconnectComponentToRelevantEntity()
        {
            world.CreateSingleton(new DisconnectClient { NetworkConnectionId = 42});

            Entity relevantEntity =
                world.EntityManager.CreateEntity(typeof(NetworkIdComponent), typeof(NetworkStreamInGame));
            world.EntityManager.SetComponentData(relevantEntity, new NetworkIdComponent { Value = 42});

            system.Update();
            world.GetExistingSystem<EndInitializationEntityCommandBufferSystem>().Update();

            Assert.IsTrue(world.EntityManager.HasComponent<NetworkStreamRequestDisconnect>(relevantEntity));
        }

        [Test]
        public void WhenConnectionDoesNotExist_ShouldRaiseError()
        {
            world.CreateSingleton(new DisconnectClient { NetworkConnectionId = 42});
            system.Update();
            world.GetExistingSystem<EndInitializationEntityCommandBufferSystem>().Update();
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();

            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(DisconnectClientFailedEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenConnectionExist_ShouldNotRaiseError()
        {
            world.CreateSingleton(new DisconnectClient { NetworkConnectionId = 42});
            Entity relevantEntity =
                world.EntityManager.CreateEntity(typeof(NetworkIdComponent), typeof(NetworkStreamInGame));
            world.EntityManager.SetComponentData(relevantEntity, new NetworkIdComponent { Value = 42});
            system.Update();
            world.GetExistingSystem<EndInitializationEntityCommandBufferSystem>().Update();
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();

            Assert.AreEqual(0, world.EntityManager.CreateEntityQuery(typeof(DisconnectClientFailedEvent)).CalculateEntityCount());
        }

        public class MyDisconnectClientSystem : DisconnectClientSystem
        {
            public bool DidUpdate;

            protected override void OnUpdate()
            {
                DidUpdate = true;
                base.OnUpdate();
            }
        }
    }
}