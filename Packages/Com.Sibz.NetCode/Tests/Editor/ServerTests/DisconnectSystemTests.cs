using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectSystemTests
    {
        private World world;
        private MyDisconnectSystem disconnectSystem;
        private BeginInitializationEntityCommandBufferSystem bufferSystem;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test_DisconnectSystem_{testCount++}");
            bufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            world.GetOrCreateSystem<EventComponentSystem>();
            disconnectSystem = world.GetOrCreateSystem<MyDisconnectSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            if (world.IsCreated)
            {
                world.Dispose();
            }
        }

        [Test]
        public void WhenSingletonDoesNotExist_ShouldNotRun()
        {
            disconnectSystem.Update();
            Assert.IsFalse(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenSingletonExist_ShouldRun()
        {
            world.CreateSingleton<Disconnect>();
            disconnectSystem.Update();
            Assert.IsTrue(disconnectSystem.DidUpdate);
        }

        [Test]
        public void WhenFirstRun_ShouldRaiseDisconnectingEvent()
        {
            world.CreateSingleton<Disconnect>();
            disconnectSystem.Update();
            bufferSystem.Update();
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(DisconnectingEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenFirstRun_ShouldCreateDestroyWorldEntity()
        {
            world.CreateSingleton<Disconnect>();
            disconnectSystem.Update();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(DestroyWorld)).CalculateEntityCount());
        }

        [Test]
        public void WhenRunTwice_ShouldDestroyDisconnectEventEntity()
        {
            Entity entity = world.CreateSingleton<Disconnect>();
            disconnectSystem.Update();
            bufferSystem.Update();
            disconnectSystem.Update();
            Assert.IsFalse(world.EntityManager.Exists(entity));
        }
    }

    public class MyDisconnectSystem : DisconnectSystem
    {
        public bool DidUpdate;

        protected override void OnUpdate()
        {
            DidUpdate = true;
            base.OnUpdate();
        }
    }
}