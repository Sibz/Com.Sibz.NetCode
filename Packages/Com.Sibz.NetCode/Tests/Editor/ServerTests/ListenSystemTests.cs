using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Tests.Server
{
    public class ListenSystemTests
    {
        private World world;
        private MyListenSystem system;
        private MyNetworkStreamProxy proxy;
        private EventComponentSystem eventSystem;
        private BeginInitializationEntityCommandBufferSystem bufferSystem;
        private int testCount;

        private EntityQuery ListenQuery =>
            world.EntityManager.CreateEntityQuery(typeof(Listen));

        [SetUp]
        public void SetUp()
        {
            world = new World($"Test_ListenSystem_{testCount++}");
            system = world.CreateSystem<MyListenSystem>();
            proxy = new MyNetworkStreamProxy();
            system.NetworkStreamProxy = proxy;
            bufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            eventSystem = world.GetOrCreateSystem<EventComponentSystem>();
        }

        [Test]
        public void WhenListenSingletonExists_ShouldRun()
        {
            world.CreateSingleton<Listen>();
            system.Update();
            Assert.IsTrue(system.DidUpdate);
        }

        [Test]
        public void WhenListenSingletonDoesNotExist_ShouldRun()
        {
            system.Update();
            Assert.IsFalse(system.DidUpdate);
        }

        [Test]
        public void WhenRun_ShouldDestroySingleton()
        {
            world.CreateSingleton<Listen>();
            system.Update();
            Assert.AreEqual(0, ListenQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenRun_ShouldCallListen()
        {
            world.CreateSingleton<Listen>();
            system.Update();
            Assert.IsTrue(((MyNetworkStreamProxy) system.NetworkStreamProxy).DidListen);
        }

        [Test]
        public void WhenListenReturnTrue_ShouldCreateListeningEntity()
        {
            world.CreateSingleton<Listen>();
            system.Update();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(Listening)).CalculateEntityCount());
        }

        [Test]
        public void WhenListenReturnTrue_ShouldCreateListeningEvent()
        {
            world.CreateSingleton<Listen>();
            system.Update();
            bufferSystem.Update();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(ListeningEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenListenReturnFalse_ShouldCreateListenFailedEvent()
        {
            world.CreateSingleton<Listen>();
            ((MyNetworkStreamProxy) system.NetworkStreamProxy).ReturnVal = false;
            system.Update();
            bufferSystem.Update();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(ListenFailedEvent)).CalculateEntityCount());
        }
    }

    public class MyListenSystem : ListenSystem
    {
        public bool DidUpdate = false;

        protected override void OnUpdate()
        {
            DidUpdate = true;
            base.OnUpdate();
        }
    }

    public class MyNetworkStreamProxy : IServerNetworkStreamProxy
    {
        public bool DidListen = false;
        public bool ReturnVal = true;

        public bool Listen(NetworkEndPoint endPoint)
        {
            DidListen = true;
            return ReturnVal;
        }
    }
}