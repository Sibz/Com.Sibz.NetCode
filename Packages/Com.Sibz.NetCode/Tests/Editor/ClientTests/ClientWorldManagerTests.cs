using System;
using NUnit.Framework;
using Sibz.NetCode.Client;
using Unity.Entities;
using Unity.Transforms;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientWorldManagerTests
    {
        private ClientWorldManager worldManager;
        private ClientOptions options;
        private int testCount;
        private EntityQuery connectingQuery;

        [SetUp]
        public void SetUp()
        {
            options = new ClientOptions() { WorldName = $"TestClient{testCount++}" };
            worldManager = new ClientWorldManager(options);
            connectingQuery = worldManager.World.EntityManager.CreateEntityQuery(typeof(Connecting));
        }

        [Test]
        public void Connect_WhenSettingIsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(()=>worldManager.Connect(null));
        }

        [Test]
        public void Connect_ShouldCreateConnectingEntity()
        {
            worldManager.Connect(options);
            Assert.AreEqual(1, connectingQuery.CalculateEntityCount());
        }
    }
}