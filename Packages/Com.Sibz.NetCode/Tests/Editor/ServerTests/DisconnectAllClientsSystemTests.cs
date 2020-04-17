using NUnit.Framework;
using Packages.Components;
using Packages.Systems;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectAllClientsSystemTests
    {
        private World world;
        private MyDisconnectAllClientsSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"testDisconnectAllClientsSystem{testCount++}");
            system = world.CreateSystem<MyDisconnectAllClientsSystem>();
        }

        [Test]
        public void WhenEntityDoesNotExist_ShouldNotRun()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenEntityExist_ShouldRun()
        {
            world.EntityManager.CreateEntity(typeof(DisconnectAllClients));
            system.Update();
            Assert.IsTrue(system.DidUpdate);
        }

        [Test]
        public void WhenRun_ShouldRemoveAllTriggerEntities()
        {
            world.EntityManager.CreateEntity(typeof(DisconnectAllClients));
            world.EntityManager.CreateEntity(typeof(DisconnectAllClients));
            system.Update();
            Assert.AreEqual(0, world.EntityManager.CreateEntityQuery(typeof(DisconnectAllClients)).CalculateEntityCount());
        }

        [Test]
        public void ShouldOnlyRunOnce()
        {
            world.EntityManager.CreateEntity(typeof(DisconnectAllClients));
            system.Update();
            system.DidUpdate = false;
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenRun_ShouldAddComponentToAllNetworkEntities()
        {
            world.EntityManager.CreateEntity(typeof(DisconnectAllClients));
            EntityArchetype at =
                world.EntityManager.CreateArchetype(typeof(NetworkIdComponent), typeof(NetworkStreamConnection));
            world.EntityManager.CreateEntity(at);
            world.EntityManager.CreateEntity(at);
            world.EntityManager.CreateEntity(at);
            system.Update();
            Assert.AreEqual(3, world.EntityManager
                .CreateEntityQuery(
                    typeof(NetworkIdComponent),
                    typeof(NetworkStreamConnection),
                    typeof(NetworkStreamRequestDisconnect)).CalculateEntityCount());
        }


        private class MyDisconnectAllClientsSystem : DisconnectAllClientsSystem
        {
            public bool DidUpdate = false;

            protected override void OnUpdate()
            {
                DidUpdate = true;
                base.OnUpdate();
            }
        }
    }
}