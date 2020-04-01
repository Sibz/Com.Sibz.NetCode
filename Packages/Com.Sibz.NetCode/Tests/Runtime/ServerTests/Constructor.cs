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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testWorld = new ServerWorld();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
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
            var state = StatusQuery.GetSingleton<NetworkStatus>().State;
            Debug.Log(state);
            Assert.AreNotEqual(NetworkState.Uninitialised, state);
        }

        [Test]
        public void Listen_ShouldUpdateStatusToListening()
        {
            testWorld.Listen();
            var state = StatusQuery.GetSingleton<NetworkStatus>().State;
            Debug.Log(state);
            Assert.AreEqual(NetworkState.Listening, state);
        }
    }
}