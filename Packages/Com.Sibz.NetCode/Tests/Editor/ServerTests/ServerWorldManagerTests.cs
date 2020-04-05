using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldManagerTests
    {
        private ServerWorldManager wm;
        private ServerOptions serverOptions;

        [SetUp]
        public void SetUp()
        {
            wm = new ServerWorldManager(new MyWorldManagerOptions());
            serverOptions = new ServerOptions();
        }

        [TearDown]
        public void TearDown()
        {
            wm.Dispose();
        }

        [Test]
        public void Listen_WhenWorldIsNotCreated_ShouldThrow()
        {
            Assert.Catch<InvalidOperationException>(() => wm.Listen(serverOptions));
        }

        [Test]
        public void Listen_WhenWorldIsCreated_ShouldReturnTrue()
        {
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            Assert.IsTrue(wm.Listen(serverOptions));
        }
    }
}