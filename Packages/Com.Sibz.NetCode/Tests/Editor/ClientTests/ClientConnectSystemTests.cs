using NUnit.Framework;
using Sibz.EntityEvents;
using Sibz.NetCode.Client;
using Sibz.NetCode.WorldExtensions;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

namespace Sibz.NetCode.Tests.Client
{
    public class ClientConnectSystemTests
    {
        private World world;
        private int testCount;
        private ClientConnectSystemTest connectSystem;
        private EventComponentSystem eventSystem;
        private BeginInitializationEntityCommandBufferSystem initBufferSystem;
        private ServerWorld testServer;

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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testServer = new ServerWorld(new ServerOptions{ WorldName = "ClientConnectTestServerWorld"});
            testServer.Listen();
        }

        [SetUp]
        public void SetUp()
        {
            world = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld,
                $"TestClientConnectSystem{testCount++}");
            connectSystem = world.CreateSystem<ClientConnectSystemTest>();
            initBufferSystem = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            eventSystem = world.CreateSystem<EventComponentSystem>();
            world.GetOrCreateSystem<ClientSimulationSystemGroup>().AddSystemToUpdateList(connectSystem);
            world.GetOrCreateSystem<ClientSimulationSystemGroup>().AddSystemToUpdateList(eventSystem);
            world.GetOrCreateSystem<ClientSimulationSystemGroup>().SortSystemUpdateList();

            world.CreateSingleton<Connecting>();
        }

        [TearDown]
        public void TearDown()
        {
            world.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            testServer.Dispose();
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
            UpdateServerAndClient();
            Assert.AreEqual(NetworkState.ConnectingToServer, State.State);
        }

        [Test]
        public void WhenConnectionProgressesFromConnectingToServer_ShouldUpdateToGoingInGame()
        {
            State = new Connecting { State = NetworkState.ConnectingToServer };
            world.GetNetworkStreamReceiveSystem().Connect(NetworkEndPoint.Parse("127.0.0.1", 21650));
            UpdateServerAndClient();
            UpdateServerAndClient();
            Assert.AreEqual(NetworkState.GoingInGame, State.State);
        }

        public void UpdateServerAndClient()
        {
            world.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            world.GetExistingSystem<ClientSimulationSystemGroup>().Update();
            testServer.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            testServer.World.GetExistingSystem<ServerSimulationSystemGroup>().Update();
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