using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientConnectSystemTests
    {
        private World world;
        private int testCount;
        private ClientConnectSystemTest connectSystem;
        private EventComponentSystem eventSystem;
        private BeginInitializationEntityCommandBufferSystem initBufferSystem;

        private EntityQuery ConnectingSingletonQuery =>
            world.EntityManager.CreateEntityQuery(typeof(Connecting));

        private EntityQuery ConnectInitEventQuery =>
            world.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent));

        private EntityQuery ConnectFailedEventQuery =>
            world.EntityManager.CreateEntityQuery(typeof(ConnectionFailedEvent));

        private Connecting State
        {
            get => ConnectingSingletonQuery.GetSingleton<Connecting>();
            set => world.EntityManager.SetComponentData(ConnectingSingletonQuery.GetSingletonEntity(), value);
        }

        [SetUp]
        public void SetUp()
        {
            world = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld,
                $"TestClientConnectSystem{testCount++}");
            connectSystem = world.CreateSystem<ClientConnectSystemTest>();
            initBufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            eventSystem = world.CreateSystem<EventComponentSystem>();

            world.CreateSingleton<Connecting>();
        }

        [TearDown]
        public void TearDown()
        {
            world.Dispose();
        }

        [Test]
        public void WhenSingletonDoesNotExist_ShouldNotRun()
        {
            world.EntityManager.DestroyEntity(ConnectingSingletonQuery.GetSingletonEntity());
            connectSystem.Update();
            Assert.IsFalse(connectSystem.Updated);
        }

        [Test]
        public void WhenSingletonExists_ShouldRun()
        {
            connectSystem.Update();
            Assert.IsTrue(connectSystem.Updated);
        }

        [Test]
        public void WhenTimeoutHasPassed_ShouldCreateFailureEvent()
        {
            connectSystem.World.EntityManager.SetComponentData(ConnectingSingletonQuery.GetSingletonEntity(),
                new Connecting { TimeoutTime = -1 });
            connectSystem.Update();
            initBufferSystem.Update();
            Assert.AreEqual(1, ConnectFailedEventQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenTimeoutHasPassed_ShouldDestroyConnectingSingleton()
        {
            State = new Connecting { TimeoutTime = -1 };
            connectSystem.Update();
            Assert.AreEqual(0, ConnectingSingletonQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenConnectionProgressesFromInitialRequest_ShouldUpdateStateToConnectingToServer()
        {
            State = new Connecting { State = NetworkState.InitialRequest };
            world.EnqueueEvent<ConnectionInitiatedEvent>();
            initBufferSystem.Update();
            connectSystem.Update();
            Assert.AreEqual(NetworkState.ConnectingToServer, State.State);
        }

        [Test]
        public void WhenConnectionProgressesFromConnectingToServer_ShouldUpdateToGoingInGame()
        {
            
        }
    }

    public class ClientConnectSystemTest : ClientConnectSystem
    {
        public bool Updated;

        protected override void OnUpdate()
        {
            Updated = true;
            base.OnUpdate();
        }
    }
}