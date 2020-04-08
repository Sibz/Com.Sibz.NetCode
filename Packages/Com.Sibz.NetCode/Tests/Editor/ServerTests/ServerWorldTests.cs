using NUnit.Framework;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldTests
    {
        private ServerWorld testWorld;

        /*private EntityQuery StatusQuery =>
            testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));*/

        private int testCount;
        [SetUp]
        public void ConstructorSetUp()
        {
         //   testWorld = new ServerWorld(new ServerOptions { WorldName = $"Server{testCount++}"});
        }

        [TearDown]
        public void ConstructorTearDown() => testWorld?.Dispose();
    }
}