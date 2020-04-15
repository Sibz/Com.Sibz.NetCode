using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Server
{
    public class ClientGoInGameSystemTests
    {
        private World world;
        private MyGoInGameSystem goInGameSystem;
        private BeginInitializationEntityCommandBufferSystem initBufferSystem;
        private EndSimulationEntityCommandBufferSystem endSimBufferSystem;
        private EntityArchetype archetype;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test{nameof(ClientGoInGameSystemTests)}{testCount++}");
            initBufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            endSimBufferSystem = world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            goInGameSystem = world.CreateSystem<MyGoInGameSystem>();
            world.CreateSystem<NetCodeEventComponentSystem>();
            archetype = world.EntityManager.CreateArchetype(typeof(GoInGameRequest),
                typeof(ReceiveRpcCommandRequestComponent));
        }

        [TearDown]
        public void TearDown()
        {
            world.Dispose();
        }

        [Test]
        public void WhenRpcEntityExist_ShouldRun()
        {
            world.EntityManager.CreateEntity(archetype);
            goInGameSystem.Update();
            Assert.IsTrue(goInGameSystem.DidUpdate);
        }

        [Test]
        public void WhenRpcEntityDoesNotExist_ShouldNotRun()
        {
            goInGameSystem.Update();
            Assert.IsFalse(goInGameSystem.DidUpdate);
        }

        [Test]
        public void WhenRun_ShouldRemoveRequestEntity()
        {
            Entity srcConnection = world.EntityManager.CreateEntity();
            Entity requestEntity = world.EntityManager.CreateEntity(archetype);
            world.EntityManager.SetComponentData(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = srcConnection });
            UpdateWorld();
            Assert.IsFalse(world.EntityManager.Exists(requestEntity));
        }

        [Test]
        public void WhenRun_ShouldAddNetworkStreamInGame()
        {
            Entity srcConnection = world.EntityManager.CreateEntity();
            Entity requestEntity = world.EntityManager.CreateEntity(archetype);
            world.EntityManager.SetComponentData(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = srcConnection });
            UpdateWorld();
            Assert.IsTrue(world.EntityManager.HasComponent<NetworkStreamInGame>(srcConnection));
        }

        [Test]
        public void WhenRun_ShouldCreateConfirmRpc()
        {
            Entity srcConnection = world.EntityManager.CreateEntity();
            Entity requestEntity = world.EntityManager.CreateEntity(archetype);
            world.EntityManager.SetComponentData(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = srcConnection });
            UpdateWorld();
            Assert.AreEqual(1,
                world.EntityManager
                    .CreateEntityQuery(typeof(ConfirmConnectionRequest), typeof(SendRpcCommandRequestComponent))
                    .CalculateEntityCount());
        }

        [Test]
        public void WhenRun_ShouldRaiseClientConnectecEvent()
        {
            Entity srcConnection = world.EntityManager.CreateEntity();
            Entity requestEntity = world.EntityManager.CreateEntity(archetype);
            world.EntityManager.SetComponentData(requestEntity,
                new ReceiveRpcCommandRequestComponent { SourceConnection = srcConnection });
            UpdateWorld();
            UpdateWorld();
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(ClientConnectedEvent)).CalculateEntityCount());
        }

        private void UpdateWorld()
        {
            initBufferSystem.Update();
            goInGameSystem.Update();
            endSimBufferSystem.Update();
        }

        public class MyGoInGameSystem : ClientGoInGameSystem
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