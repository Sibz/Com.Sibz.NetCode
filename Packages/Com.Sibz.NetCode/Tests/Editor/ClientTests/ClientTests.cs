using NUnit.Framework;
using Sibz.NetCode.Client;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientTests
    {
        private World world;
        private MyClientWorld clientWorld;
        private static int testCount;

        [SetUp]
        public void SetUp()
        {
            clientWorld = new MyClientWorld();
            world = clientWorld.World;
        }

        [Test]
        public void Disconnect_ShouldCreateSingleton()
        {
            clientWorld.Disconnect();
            Assert.AreEqual(1, world.EntityManager.CreateEntityQuery(typeof(Disconnect)).CalculateEntityCount());
        }

        private class MyClientWorld : ClientWorld
        {
            public MyClientWorld() : base(GetOptions())
            {
            }
        }

        private static ClientOptions GetOptions()
        {
            return new ClientOptions
            {
                WorldName = $"TestClientWorld{testCount++}"
            };
        }
    }
}