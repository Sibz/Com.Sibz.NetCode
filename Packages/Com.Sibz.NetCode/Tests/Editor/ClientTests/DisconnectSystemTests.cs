using NUnit.Framework;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Client
{
    public class DisconnectSystemTests
    {
        private World world;
        private MyDisconnectSystem disconnectSystem;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test{nameof(DisconnectSystem)}{testCount++}");
            UpdateWorld();
            world.CreateSystem<NetCodeEventComponentSystem>();
            disconnectSystem = world.CreateSystem<MyDisconnectSystem>();
        }

        [Test]
        public void WhenSingletonExist_ShouldRun()
        {
            world.CreateSingleton<Disconnect>();
            disconnectSystem.Update();
            Assert.IsTrue(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenSingletonDoesNotExist_ShouldNotRun()
        {
            disconnectSystem.Update();
            Assert.IsFalse(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenRunAndNotInGame_ShouldDestroySingleton()
        {
            Entity singleton = world.CreateSingleton<Disconnect>();
            UpdateWorld();
            Assert.IsFalse(world.EntityManager.Exists(singleton));
        }

        [Test]
        public void WhenFirstRun_ShouldCreateEvent()
        {
            world.CreateSingleton<Disconnect>();
            world.CreateSingleton<NetworkStreamInGame>();
            UpdateWorld();
            UpdateWorld();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(DisconnectedEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenRunTwice_ShouldDestroySingleton()
        {
            Entity entity = world.CreateSingleton<Disconnect>();
            world.CreateSingleton<NetworkStreamInGame>();
            UpdateWorld();
            UpdateWorld();
            Assert.IsFalse(world.EntityManager.Exists(entity));
        }

        [Test]
        public void WhenRunTwice_ShouldAddComponentToStreamEntity()
        {
            world.CreateSingleton<Disconnect>();
            Entity entity = world.CreateSingleton<NetworkStreamInGame>();
            UpdateWorld();
            UpdateWorld();
            Assert.IsTrue(world.EntityManager.HasComponent<NetworkStreamRequestDisconnect>(entity));
        }

        private void UpdateWorld()
        {
            world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            disconnectSystem?.Update();
            world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().Update();
        }

        private class MyDisconnectSystem : DisconnectSystem
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