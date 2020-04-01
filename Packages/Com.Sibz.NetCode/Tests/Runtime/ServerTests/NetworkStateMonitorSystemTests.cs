using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Server
{
    public class NetworkStateMonitorSystemTests : TestBase
    {
        private ServerWorld serverWorld;

        private EntityQuery NetworkStatusQuery => GetSingletonQuery<NetworkStatus>();


        private EntityQuery GetSingletonQuery<T>()
            where T : struct, IComponentData=>
            serverWorld.World.EntityManager.CreateEntityQuery(typeof(T));

        private NetworkStatus NetworkStatus => NetworkStatusQuery.GetSingleton<NetworkStatus>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
            serverWorld = new ServerWorld(new ServerOptions() {Port = 20250});
        }

        [TearDown]
        public void TearDown()
        {
            serverWorld.Dispose();
        }

        public void OneTimeTearDown()
        {
        }

        [Test]
        public void WhenDisconnected_ShouldUpdateStateToDisconnection()
        {
            serverWorld.Listen();
            if (NetworkStatus.State != NetworkState.Listening)
            {
                Assert.Fail($"Server state should be listening for this test, was {NetworkStatus.State}");
            }
            serverWorld.Disconnect();
            serverWorld.World.GetExistingSystem<NetworkStateMonitorSystem>().Update();
            Assert.AreEqual(NetworkState.Disconnected, NetworkStatus.State);
        }

        [Test]
        public void ShouldBeAbleToListenAgainAfterDisconnect()
        {
            serverWorld.Listen();
            serverWorld.Disconnect();
            serverWorld.World.GetExistingSystem<NetworkStateMonitorSystem>().Update();
            serverWorld.Listen();
            Assert.AreEqual(NetworkState.Listening, NetworkStatus.State);
        }

        [Test]
        public void WhenNoClientsConnected_ShouldHaveZeroConnectionCount()
        {
            serverWorld.Listen();
            serverWorld.World.GetExistingSystem<NetworkStateMonitorSystem>().Update();
            Assert.AreEqual(0, NetworkStatus.ConnectionCount);
        }

        [Test]
        public void WhenNetworksStateChanges_ShouldCreateEvent()
        {
            if (NetworkStatus.State != NetworkState.Uninitialised)
            {
                Assert.Fail($"Server state should be Uninitialised for this test, was {NetworkStatus.State}");
            }
            serverWorld.Listen();
            // Detect change create eventity next frame
            serverWorld.World.GetExistingSystem<NetworkStateMonitorSystem>().Update();
            serverWorld.World.GetExistingSystem<EventComponentSystem>().Update();
            serverWorld.World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            //UpdateWorld(serverWorld); // Eventity added

            Assert.AreEqual(1, GetSingletonQuery<NetworkStateChangeEvent>().CalculateEntityCount());

        }

        [Test]
        public void WhenClientIsNotInGame_ShouldHaveCorrectInGameCount()
        {
            Assert.Fail();
        }

        private void UpdateWorld(ServerWorld world)
        {
            world.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            world.World.GetExistingSystem<ServerSimulationSystemGroup>().Update();
        }
    }
}