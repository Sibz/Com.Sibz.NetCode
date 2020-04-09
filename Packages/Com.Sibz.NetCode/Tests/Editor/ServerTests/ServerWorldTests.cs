using NUnit.Framework;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldTests
    {
        private MyServerWorld testWorld;
        private ServerOptions serverOptions;

        /*private EntityQuery StatusQuery =>
            testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));*/

        private int testCount;

        [SetUp]
        public void SetUp()
        {
            serverOptions = new ServerOptions
            {
                WorldName = $"TestServerWorld{testCount++}"
            };
            testWorld = new MyServerWorld(serverOptions);
        }

        [TearDown]
        public void TearDown() => testWorld?.Dispose();

        [Test]
        public void ShouldHaveSystemsInCreatedWorld()
        {
            testWorld.CreateWorld();
            Assert.IsNotNull(testWorld.World.GetExistingSystem<NetCodeEventComponentSystem>());
            Assert.IsNotNull(testWorld.World.GetExistingSystem<NetCodeHookSystem>());
        }

        private class MyServerWorld : ServerWorld
        {
            public MyServerWorld(ServerOptions options) : base(options)
            {
            }

            public new void CreateWorld() => base.CreateWorld();
        }
    }


}