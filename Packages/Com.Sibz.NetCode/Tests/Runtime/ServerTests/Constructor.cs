using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;
using UnityEngine;

namespace Sibz.NetCode.Tests.Server
{
    public class Constructor : TestBase
    {
        private ServerWorld testWorld;

        private EntityQuery StatusQuery =>
            testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));

        [SetUp]
        public void ConstructorSetUp()
        {
            testWorld = new ServerWorld();
        }

        [TearDown]
        public void ConstructorTearDown()
        {
            testWorld.Dispose();
        }

        [Test]
        public void ShouldCreateStatusEntity()
        {
            Assert.AreEqual(1, StatusQuery.CalculateEntityCount());
        }

        [Test]
        public void Listen_ShouldUpdateStatus()
        {
            testWorld.Listen();
            NetworkState state = StatusQuery.GetSingleton<NetworkStatus>().State;
            Debug.Log(state);
            Assert.AreNotEqual(NetworkState.Uninitialised, state);
        }

        [Test]
        public void Listen_ShouldUpdateStatusToListening()
        {
            testWorld.Listen();
            NetworkState state = StatusQuery.GetSingleton<NetworkStatus>().State;
            Debug.Log(state);
            Assert.AreEqual(NetworkState.Listening, state);
        }

        [Test]
        public void WhenListening_ShouldCreateNetworkStateChangeEvent()
        {
            testWorld.Listen();
            testWorld.World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>().Update();
            Assert.AreEqual(1,
                testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStateChangeEvent)).CalculateEntityCount());
        }
    }
}