using NUnit.Framework;
using Sibz.NetCode.Server;

namespace Sibz.NetCode.Tests.Server
{
    public class NetworkStateMonitorSystemTests : TestBase
    {
        private ServerWorld serverWorld;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [SetUp]
        public void SetUp()
        {
            serverWorld = new ServerWorld();
        }

        [TearDown]
        public void TearDown()
        {
            serverWorld.Dispose();
        }
        [Test]
        public void WhenDisconnected_ShouldUpdateStateToDisconnection()
        {
            serverWorld.Disconnect();
            Assert.Fail();
        }

        [Test]
        public void WhenClientIsConnected_ShouldHaveCorrectConnectionCount()
        {
            Assert.Fail();
        }
        [Test]
        public void WhenClientIsInGame_ShouldHaveCorrectInGameCount()
        {
            Assert.Fail();
        }
    }
}