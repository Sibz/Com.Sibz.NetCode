using NUnit.Framework;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

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
            world.CreateSystem<EndInitializationEntityCommandBufferSystem>();
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

        [Test]
        public void ShouldAddDisconnectComponentToRelevantEntity()
        {
            world.CreateSingleton(new DisconnectClient { NetworkConnectionId = 42});
            Entity relevantEntity = world.CreateSingleton(new NetworkIdComponent { Value = 42 });

            system.Update();
            world.GetExistingSystem<EndInitializationEntityCommandBufferSystem>().Update();

            Assert.IsTrue(world.EntityManager.HasComponent<NetworkStreamRequestDisconnect>(relevantEntity));
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