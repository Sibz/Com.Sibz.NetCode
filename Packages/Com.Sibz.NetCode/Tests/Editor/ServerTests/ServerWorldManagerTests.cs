using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Networking.Transport;

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

        [Test]
        public void Listen_ShouldSetIsListeningToTrue()
        {
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);
            Assert.IsTrue(wm.IsListening);
        }

        [Test]
        public void Listen_ShouldInvokeCallback()
        {
            bool success = false;
            void OnListenSuccess()
            {
                success = true;
            }
            wm.CallbackProvider = new MyServerCallbackProvider { ListenSuccess = OnListenSuccess };
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);
            Assert.IsTrue(success);
        }

        public class MyServerCallbackProvider: IServerWorldCallbackProvider
        {
            public Action WorldCreated { get; set; }
            public Action WorldDestroyed { get; set; }
            public Action PreWorldDestroy { get; set; }
            public Action<NetworkConnection> ClientConnected { get; set; }
            public Action<NetworkConnection> ClientDisconnected { get; set; }
            public Action ListenSuccess { get; set; }
            public Action ListenFailed { get; set; }
            public Action Closed { get; set; }
        }
    }
}