using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.NetCode.Server;
using Sibz.NetCode.WorldExtensions;

namespace Sibz.NetCode.Tests.Server
{
    public class ServerWorldTests
    {
        private MyServerWorld testWorld;
        private ServerOptions serverOptions;

        /*private EntityQuery StatusQuery =>
            testWorld.World.EntityManager.CreateEntityQuery(typeof(NetworkStatus));*/

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
            Assert.AreEqual(1, testWorld.World.EntityManager.CreateEntityQuery(typeof(Disconnect)).CalculateEntityCount());
        }

        [Test]
        public void WhenClientConnectedEventRaised_ShouldCallback()
        {
            testWorld.World.CreateSingleton<ClientConnectedEvent>();
            testWorld.World.GetHookSystem().Update();
            Assert.AreEqual(MyServerWorld.CallbackName.ClientConnected, testWorld.LastCallbackName);
        }



        private class MyServerWorld : ServerWorld
        {
            public CallbackName LastCallbackName= CallbackName.None;
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