using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldManagerTests
    {
        private ServerWorldManager wm;
        private ServerOptions serverOptions;
        private readonly List<Type> systems = new List<Type>();

        [SetUp]
        public void SetUp()
        {
            wm = new ServerWorldManager(new MyWorldManagerOptions());
            serverOptions = new ServerOptions();
            systems.AppendTypesWithAttribute<ServerSystemAttribute>();
            systems.AppendTypesWithAttribute<ClientAndServerSystemAttribute>();
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
        public void Listen_ShouldCreateSingleton()
        {
            wm.CreateWorld(systems);
            wm.Listen(serverOptions);
            Assert.AreEqual(1, wm.World.EntityManager.CreateEntityQuery(typeof(Listen)).CalculateEntityCount());
        }

        [Test]
        public void WhenSettingNull_ShouldThrow()
        {
            wm.CreateWorld(systems);
            Assert.Catch<ArgumentNullException>(() => wm.Listen(null));
        }

        [Test]
        public void ShouldSetEndPoint()
        {
            wm.CreateWorld(systems);
            wm.Listen(serverOptions);
            var listen = wm.World.EntityManager.CreateEntityQuery(typeof(Listen)).GetSingleton<Listen>();
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(serverOptions.Address, serverOptions.Port, serverOptions.NetworkFamily);
            Assert.AreEqual(listen.EndPoint, endPoint);
        }

        /*
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

        /*
        [Test]
        public void Listen_WhenPortTaken_ShouldFailAndInvokeCallback()
        {
            bool success = false;

            void OnListenFailed()
            {
                success = true;
            }

            var wm2 = new ServerWorldManager(new MyWorldManagerOptions());
            wm2.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm2.Listen(serverOptions);
            wm.CallbackProvider = new MyServerCallbackProvider { ListenFailed = OnListenFailed };
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);
            wm2.Dispose();
            Assert.IsTrue(success);
        }
        #1#

        [Test]
        public void Close_WhenWorldNotCreated_ShouldNotThrow()
        {
            wm.Close();
        }

        [Test]
        public void Close_WhenWorldNotListening_ShouldNotThrow()
        {
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Close();
        }

        [Test]
        public void Close_WhenListening_ShouldDestroyWorld()
        {
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);
            wm.Close();
            Assert.IsFalse(wm.World.IsCreated);
        }

        [Test]
        public void Close_WhenListening_ShouldSetListeningToFalse()
        {
            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);

            Assert.IsTrue(wm.IsListening, "Listening should be set to true for this test");

            wm.Close();

            Assert.IsFalse(wm.IsListening);
        }

        [Test]
        public void Close_WhenListening_ShouldInvokeCallback()
        {
            bool success = false;

            void OnClose()
            {
                success = true;
            }

            wm.CreateWorld(new List<Type> { typeof(NetCodeEventComponentSystem) });
            wm.Listen(serverOptions);
            wm.CallbackProvider = new MyServerCallbackProvider { Closed = OnClose };
            wm.Close();

            Assert.IsTrue(success);
        }*/
    }
}