using NUnit.Framework;
using Sibz.EntityEvents;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectSystemTests
    {
        private World world;
        private MyDisconnectSystem disconnectSystem;
        private BeginInitializationEntityCommandBufferSystem bufferSystem;
        private EventComponentSystem eventComponentSystem;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test_DisconnectSystem_{testCount++}");
            bufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            eventComponentSystem = world.GetOrCreateSystem<EventComponentSystem>();
            disconnectSystem = world.GetOrCreateSystem<MyDisconnectSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            world.Dispose();
        }

        [Test]
        public void WhenSingletonDoesNotExist_ShouldNotRun()
        {
            disconnectSystem.Update();
            Assert.IsFalse(disconnectSystem.DidUpdate);
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