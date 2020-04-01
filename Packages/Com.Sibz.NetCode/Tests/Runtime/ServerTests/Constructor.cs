using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class Constructor : TestBase
    {
        [Test]
        public void ShouldCreateStatusEntity()
        {
            ServerWorld world = new ServerWorld();
            EntityQuery q = world.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));
            Assert.AreEqual(1, q.CalculateEntityCount());
        }
    }
}