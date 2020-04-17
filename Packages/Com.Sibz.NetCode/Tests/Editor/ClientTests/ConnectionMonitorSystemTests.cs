using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Client
{
    public class ConnectionMonitorSystemTests
    {
        private World world;
        private MyConnectionMonitorSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"TestCMS${testCount++}");
            world.CreateSystem<BeginInitializationEntityCommandBufferSystem>();
            world.CreateSystem<EventComponentSystem>();
            world.CreateSystem<NetCodeHookSystem>();
            system = world.CreateSystem<MyConnectionMonitorSystem>();
        }

        private void RaiseConnectionEvent()
        {
            world.EnqueueEvent<ConnectionCompleteEvent>();
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            world.GetHookSystem().Update();
        }

        [Test]
        public void WhenConnected_ShouldRun()
        {
            RaiseConnectionEvent();
            system.Update();
            Assert.IsTrue(system.DidUpdate);
        }

        [Test]
        public void WhenNotYetConnected_ShouldNotRun()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        private Entity CreateEntityAndRun()
        {
            Entity entity =
                world.EntityManager.CreateEntity(typeof(NetworkIdComponent), typeof(NetworkStreamConnection));
            RaiseConnectionEvent();
            system.Update();
            return entity;
        }

        private void DestroyEntityAndRun(Entity entity)
        {
            world.EntityManager.DestroyEntity(entity);
            system.Update();
            world.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
        }

        [Test]
        public void WhenConnectionComesAndThenGoes_ShouldRaiseDisconnectedEvent()
        {
            DestroyEntityAndRun(CreateEntityAndRun());
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(DisconnectedEvent)).CalculateEntityCount());
        }

        [Test]
        public void ShouldNotRunAfterRaisingEvent()
        {
            DestroyEntityAndRun(CreateEntityAndRun());
            system.DidUpdate = false;
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        private class MyConnectionMonitorSystem : ConnectionMonitorSystem
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