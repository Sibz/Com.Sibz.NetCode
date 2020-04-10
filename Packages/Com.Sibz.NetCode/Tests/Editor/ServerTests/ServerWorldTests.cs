using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldTests
    {
        private MyServerWorld testWorld;
        private ServerOptions serverOptions;

        private int testCount;

        [SetUp]
        public void SetUp()
        {
            serverOptions = new ServerOptions
            {
                WorldName = $"TestServerWorld{testCount++}"
            };
            testWorld = new MyServerWorld(serverOptions);
            testWorld.CreateWorld();
        }

        [TearDown]
        public void TearDown() => testWorld?.Dispose();

        [Test]
        public void ShouldHaveSystemsInCreatedWorld()
        {
            //testWorld.CreateWorld();
            List<Type> systems = new List<Type>().AppendTypesWithAttribute<ServerSystemAttribute>();
            foreach (Type system in systems)
            {
                Assert.IsNotNull(testWorld.World.GetExistingSystem(system), $"System: {system.Name}");
            }
        }

        [Test]
        public void Close_ShouldCreateSingleton()
        {
            //testWorld.CreateWorld();
            testWorld.Close();
            Assert.AreEqual(1,
                testWorld.World.EntityManager.CreateEntityQuery(typeof(Disconnect)).CalculateEntityCount());
        }

        [Test]
        public void WhenClientConnectedEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<ClientConnectedEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.ClientConnected, testWorld.LastCallbackName);
        }

        [Test]
        public void WhenClientDisconnectedEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<ClientDisconnectedEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.ClientDisconnected, testWorld.LastCallbackName);
        }

        [Test]
        public void WhenListeningEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<ListeningEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.ListenSuccess, testWorld.LastCallbackName);
        }

        [Test]
        public void WhenListenFailedEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<ListenFailedEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.ListenFailed, testWorld.LastCallbackName);
        }

        [Test]
        public void WhenDisconnectingEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<DisconnectingEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.Closed, testWorld.LastCallbackName);
        }

        [Test]
        public void Listen_WhenWorldIsNotCreated_ShouldCreateWorld()
        {
            testWorld.Dispose();
            testWorld.Listen();
            Assert.IsNotNull(testWorld.World);
            Assert.IsTrue(testWorld.World.IsCreated);
        }

        [Test]
        public void Listen_ShouldCreateSingleton()
        {
            testWorld.Listen();
            Assert.AreEqual(1, testWorld.World.EntityManager.CreateEntityQuery(typeof(Listen)).CalculateEntityCount());
        }

        [Test]
        public void Listen_ShouldSetEndPoint()
        {
            testWorld.Listen();
            var listen = testWorld.World.EntityManager.CreateEntityQuery(typeof(Listen)).GetSingleton<Listen>();
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(serverOptions.Address, serverOptions.Port, serverOptions.NetworkFamily);
            Assert.AreEqual(listen.EndPoint, endPoint);
        }

        private class MyServerWorld : ServerWorld
        {
            public CallbackName LastCallbackName = CallbackName.None;

            public enum CallbackName
            {
                None,
                ClientConnected,
                ClientDisconnected,
                ListenSuccess,
                ListenFailed,
                Closed
            }

            public MyServerWorld(ServerOptions options) : base(options)
            {
                ClientConnected += (ent) => LastCallbackName = CallbackName.ClientConnected;
                ClientDisconnected += (i) => LastCallbackName = CallbackName.ClientDisconnected;
                ListenSuccess += () => LastCallbackName = CallbackName.ListenSuccess;
                ListenFailed += () => LastCallbackName = CallbackName.ListenFailed;
                Closed += () => LastCallbackName = CallbackName.Closed;
            }

            public new void CreateWorld() => base.CreateWorld();
        }
    }
}