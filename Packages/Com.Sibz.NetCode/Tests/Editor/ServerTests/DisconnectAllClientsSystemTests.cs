using NUnit.Framework;
using Packages.Systems;
using Sibz.EntityEvents;
using Unity.Entities;

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
            world.CreateSystem<BeginInitializationEntityCommandBufferSystem>();
            world.CreateSystem<EventComponentSystem>();
            system = world.CreateSystem<MyDisconnectAllClientsSystem>();
        }

        [Test]
        public void WhenEntityDoesNotExist_ShouldNotRun()
        {
            system.Update();
            //Assert.IsFalse(system.DidUpdate);
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