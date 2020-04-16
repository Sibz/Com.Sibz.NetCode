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
        public void TearDown()
        {
            testWorld?.Dispose();
        }

        [Test]
        public void WhenCreatedWithOptionsNull_ShouldThrow()
        {
            Assert.Catch<ArgumentNullException>(() => new ServerWorld(null));
        }

        [Test]
        public void ShouldHaveSystemsInCreatedWorld()
        {
            List<Type> systems = new List<Type>().AppendTypesWithAttribute<ServerSystemAttribute>();
            foreach (Type system in systems)
            {
                Assert.IsNotNull(testWorld.World.GetExistingSystem(system), $"System: {system.Name}");
            }
        }

        [Test]
        public void Close_ShouldCreateSingleton()
        {
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
        public void WhenWorldCreatedOnInitiate_ShouldStillCallCallbacks()
        {
            testWorld.Dispose();
            testWorld = new MyServerWorld(new ServerOptions
            {
                WorldName = $"TestServerCreatedOnInitiate{testCount++}",
                CreateWorldOnInstantiate = true
            });
            WhenClientConnectedEventRaised_ShouldCallback();
            WhenClientDisconnectedEventRaised_ShouldCallback();
            WhenListeningEventRaised_ShouldCallback();
            WhenListenFailedEventRaised_ShouldCallback();
            WhenDisconnectingEventRaised_ShouldCallback();
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
            Listen listen = testWorld.World.EntityManager.CreateEntityQuery(typeof(Listen)).GetSingleton<Listen>();
            NetworkEndPoint endPoint =
                NetworkEndPoint.Parse(serverOptions.Address, serverOptions.Port, serverOptions.NetworkFamily);
            Assert.AreEqual(listen.EndPoint, endPoint);
        }

        [Test]
        public void DisconnectClient_ShouldCreateEntity()
        {
            testWorld.DisconnectClient(42);
            Assert.AreEqual(1, testWorld.World.EntityManager.CreateEntityQuery(typeof(DisconnectClient)).CalculateEntityCount());
        }

        private class MyServerWorld : ServerWorld
        {
            public enum CallbackName
            {
                None,
                ClientConnected,
                ClientDisconnected,
                ListenSuccess,
                ListenFailed,
                Closed
            }

            public CallbackName LastCallbackName = CallbackName.None;

            public MyServerWorld(ServerOptions options) : base(options)
            {
                ClientConnected += ent => LastCallbackName = CallbackName.ClientConnected;
                ClientDisconnected += i => LastCallbackName = CallbackName.ClientDisconnected;
                ListenSuccess += () => LastCallbackName = CallbackName.ListenSuccess;
                ListenFailed += () => LastCallbackName = CallbackName.ListenFailed;
                Closed += () => LastCallbackName = CallbackName.Closed;
            }

            public new void CreateWorld()
            {
                base.CreateWorld();
            }
        }
    }
}