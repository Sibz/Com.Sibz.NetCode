using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Server
{
    public class ClientDisconnectionSystemTests
    {
        private World world;
        private MyClientDisconnectSystem disconnectSystem;
        private int testCount;
        private EntityArchetype disconnectArchetype;


        [SetUp]
        public void SetUp()
        {
            world = new World($"Test{nameof(ClientDisconnectSystem)}{testCount++}");
            world.CreateSystem<EndSimulationEntityCommandBufferSystem>();
            world.CreateSystem<BeginInitializationEntityCommandBufferSystem>();
            world.CreateSystem<NetCodeEventComponentSystem>();
            disconnectSystem = world.CreateSystem<MyClientDisconnectSystem>();
            disconnectArchetype = world.EntityManager.CreateArchetype(typeof(NetworkStreamDisconnected),
                typeof(NetworkIdComponent), typeof(CommandTargetComponent));
        }

        [Test]
        public void WhenConnectionWithDisconnectDoesNotExist_ShouldNotRun()
        {
            UpdateWorld();
            Assert.IsFalse(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenConnectionWithDisconnectExist_ShouldRun()
        {
            CreateFakeDisconnectionEntity();
            UpdateWorld();
            Assert.IsTrue(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenTargetEntityIsSet_ShouldDestroy()
        {
            Entity targetEntity = world.EntityManager.CreateEntity();
            CreateFakeDisconnectionEntity(targetEntity);
            UpdateWorld();
            Assert.IsFalse(world.EntityManager.Exists(targetEntity));
        }

        [Test]
        public void WhenRun_ShouldRaiseEvent()
        {
            CreateFakeDisconnectionEntity();
            UpdateWorld();
            UpdateWorld();
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(ClientDisconnectedEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenRaisingEvent_ShouldSetNetworkId()
        {
            CreateFakeDisconnectionEntity();
            UpdateWorld();
            UpdateWorld();
            ClientDisconnectedEvent ev = world.EntityManager
                .CreateEntityQuery(typeof(ClientDisconnectedEvent)).GetSingleton<ClientDisconnectedEvent>();
            Assert.AreEqual(42, ev.NetworkId);
        }

        private Entity CreateFakeDisconnectionEntity(Entity targetEntity = default)
        {
            Entity entity = world.EntityManager.CreateEntity(disconnectArchetype);
            world.EntityManager.SetComponentData(entity, new CommandTargetComponent { targetEntity = targetEntity });
            world.EntityManager.SetComponentData(entity, new NetworkIdComponent { Value = 42 });
            return entity;
        }

        private void UpdateWorld()
        {
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            disconnectSystem.Update();
            world.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().Update();
        }

        private class MyClientDisconnectSystem : ClientDisconnectSystem
        {
            public bool DidUpdate;

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                DidUpdate = true;
                return base.OnUpdate(inputDeps);
            }
        }
    }
}