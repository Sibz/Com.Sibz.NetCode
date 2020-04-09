using System;
using System.Collections.Generic;
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
            List<Type> systems = new List<Type>().AppendTypesWithAttribute<ServerSystemAttribute>();
            foreach (Type system in systems)
            {
                Assert.IsNotNull(testWorld.World.GetExistingSystem(system), $"System: {system.Name}");
            }
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