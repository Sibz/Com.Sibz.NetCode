using System.Threading.Tasks;
using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Base
{
    public class DestroyWorldSystemTests
    {
        private World world;
        private MyDestroyWorldSystem destroyWorldSystem;
        private BeginInitializationEntityCommandBufferSystem bufferSystem;
        private EventComponentSystem eventComponentSystem;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test_DisconnectSystem_{testCount++}");
            bufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            eventComponentSystem = world.GetOrCreateSystem<EventComponentSystem>();
            destroyWorldSystem = world.GetOrCreateSystem<MyDestroyWorldSystem>();
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
            destroyWorldSystem.Update();
            Assert.IsFalse(destroyWorldSystem.DidUpdate);
        }

        [Test]
        public void WhenSingletonExist_ShouldRun()
        {
            world.CreateSingleton<DestroyWorld>();
            destroyWorldSystem.Update();
            Assert.IsTrue(destroyWorldSystem.DidUpdate);
        }

        [Test]
        public void WhenFirstRun_ShouldRaiseDisconnectingEvent()
        {
            world.CreateSingleton<DestroyWorld>();
            destroyWorldSystem.Update();
            bufferSystem.Update();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(DestroyWorldEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenFirstRun_ShouldNotDisposeWorld()
        {
            world.CreateSingleton<DestroyWorld>();
            destroyWorldSystem.Update();
            Assert.IsTrue(world.IsCreated);
        }


        [Test]
        public void WhenRunTwice_ShouldDisposeWorld()
        {
            world.CreateSingleton<DestroyWorld>();
            destroyWorldSystem.Update();
            bufferSystem.Update();
            destroyWorldSystem.Update();
            Task.Delay(10).Wait();
            Assert.IsFalse(world.IsCreated);
        }

        [Test]
        public void WhenRunTwice_ShouldDestroyDisconnectEventEntity()
        {
            Entity entity = world.CreateSingleton<DestroyWorld>();
            destroyWorldSystem.Update();
            bufferSystem.Update();
            destroyWorldSystem.Update();
            Assert.IsFalse(world.EntityManager.Exists(entity));
        }
    }

    public class MyDestroyWorldSystem : DestroyWorldSystem
    {
        public bool DidUpdate;
        protected override void OnUpdate()
        {
            DidUpdate = true;
            base.OnUpdate();
        }
    }
}