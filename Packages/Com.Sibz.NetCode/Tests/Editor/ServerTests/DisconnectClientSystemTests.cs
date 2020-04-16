using NUnit.Framework;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectClientSystemTests
    {
        private World world;
        private MyDisconnectClientSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"TestDisconnectClientSystem{testCount++}");
            system = world.CreateSystem<MyDisconnectClientSystem>();
        }

        [Test]
        public void WhenDisconnectClientEntityDoesNotExist_ShouldNotRun()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenDisconnectClientEntityExist_ShouldRun()
        {
            world.CreateSingleton<DisconnectClient>();
            system.Update();
            Assert.IsTrue(system.DidUpdate);
        }

        public class MyDisconnectClientSystem : DisconnectClientSystem
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