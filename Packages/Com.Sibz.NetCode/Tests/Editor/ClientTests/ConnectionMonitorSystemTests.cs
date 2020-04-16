using NUnit.Framework;
using Packages.Com.Sibz.NetCode.Client.Runtime.Systems;
using Sibz.EntityEvents;
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
            system = world.CreateSystem<MyConnectionMonitorSystem>();
        }
        [Test]
        public void WhenConnectionExist_ShouldRun()
        {
            world.EntityManager.CreateEntity(typeof(NetworkIdComponent), typeof(NetworkStreamConnection));
        }


        private class MyConnectionMonitorSystem : ConnectionMonitorSystem
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