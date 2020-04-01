using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class NetworkStateMonitorSystemTests : TestBase
    {
        private ServerWorld serverWorld;

        private EntityQuery NetworkStatusQuery =>
            serverWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));

        private NetworkStatus NetworkStatus => NetworkStatusQuery.GetSingleton<NetworkStatus>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [SetUp]
        public void SetUp()
        {
            serverWorld = new ServerWorld(new ServerOptions() { Port = 20250});
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
        public void WhenClientIsNotInGame_ShouldHaveCorrectInGameCount()
        {
            Assert.Fail();
        }
    }
}