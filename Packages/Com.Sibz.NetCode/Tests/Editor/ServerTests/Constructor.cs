﻿using NUnit.Framework;

namespace Sibz.NetCode.Tests.Server
{
    public class Constructor : TestBase
    {
        private ServerWorld testWorld;

        /*private EntityQuery StatusQuery =>
            testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));*/

        [SetUp]
        public void ConstructorSetUp()
        {
            //testWorld = new ServerWorld();
        }

        [TearDown]
        public void ConstructorTearDown() => testWorld?.Dispose();
    }
}