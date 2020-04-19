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

        private EntityQuery NetworkIdQuery =>
            world.EntityManager.CreateEntityQuery(typeof(NetworkIdComponent));

        private EntityQuery GoInGameRequestQuery =>
            world.EntityManager.CreateEntityQuery(typeof(GoInGameRequest), typeof(SendRpcCommandRequestComponent));

        private EntityQuery ConnectSuccessEventQuery =>
            world.EntityManager.CreateEntityQuery(typeof(ConnectionCompleteEvent));

        private Connecting State
        {
            get => ConnectingSingletonQuery.GetSingleton<Connecting>();
            set => world.EntityManager.SetComponentData(ConnectingSingletonQuery.GetSingletonEntity(), value);
        }

        [SetUp]
        public void SetUp()
        {
            world = new World(
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

        [Test]
        public void WhenSingletonDoesNotExist_ShouldNotRun()
        {
            world.EntityManager.DestroyEntity(ConnectingSingletonQuery.GetSingletonEntity());
            UpdateClient();
            Assert.IsFalse(connectSystem.Updated);
        }

        [Test]
        public void WhenSingletonExists_ShouldRun()
        {
            UpdateClient();
            Assert.IsTrue(connectSystem.Updated);
        }

        [Test]
        public void WhenTimeoutHasPassed_ShouldCreateFailureEvent()
        {
            connectSystem.World.EntityManager.SetComponentData(ConnectingSingletonQuery.GetSingletonEntity(),
                new Connecting { Timeout = -1 });
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(1, ConnectFailedEventQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenTimeoutHasPassed_ShouldDestroyConnectingSingleton()
        {
            State = new Connecting { Timeout = -1 };
            UpdateClient();
            Assert.AreEqual(0, ConnectingSingletonQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenInitialRequest_ShouldCreateEvent()
        {
            State = new Connecting { State = NetworkState.InitialRequest };
            connectSystem.NetworkStreamSystemProxy = new MyClientNetworkStreamSystemProxy();
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(1,
                world.EntityManager.CreateEntityQuery(typeof(ConnectionInitiatedEvent)).CalculateEntityCount());
        }

        [Test]
        public void WhenConnectionProgressesFromInitialRequest_ShouldUpdateStateToConnectingToServer()
        {
            State = new Connecting { State = NetworkState.InitialRequest, Timeout = 10};
            world.EnqueueEvent<ConnectionInitiatedEvent>();
            connectSystem.NetworkStreamSystemProxy = new MyClientNetworkStreamSystemProxy();
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(NetworkState.ConnectingToServer, State.State);
        }


        [Test]
        public void WhenConnectionProgressesFromConnectingToServer_ShouldUpdateToGoingInGame()
        {
            State = new Connecting { State = NetworkState.ConnectingToServer };
            world.CreateSingleton<NetworkIdComponent>();
            UpdateClient();
            Assert.AreEqual(NetworkState.GoingInGame, State.State);
        }

        [Test]
        public void WhenConnected_ShouldAddNetworkInStreamComponent()
        {
            State = new Connecting { State = NetworkState.ConnectingToServer };
            world.CreateSingleton<NetworkIdComponent>();
            UpdateClient();
            Assert.IsTrue(
                world.EntityManager.HasComponent<NetworkStreamInGame>(NetworkIdQuery.GetSingletonEntity()));
        }

        [Test]
        public void WhenConnected_ShouldSendGoInGameRequest()
        {
            State = new Connecting { State = NetworkState.ConnectingToServer };
            world.CreateSingleton<NetworkIdComponent>();
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(1, GoInGameRequestQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenReceivedConnectConfirmation_ShouldRaiseConnectCompleteEvent()
        {
            State = new Connecting { State = NetworkState.GoingInGame };
            world.EntityManager.CreateEntity(typeof(ConfirmConnectionRequest),
                typeof(ReceiveRpcCommandRequestComponent));
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(1, ConnectSuccessEventQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenReceivedConnectConfirmation_ShouldDestroyConnectingComponent()
        {
            State = new Connecting { State = NetworkState.GoingInGame };
            world.EntityManager.CreateEntity(typeof(ConfirmConnectionRequest),
                typeof(ReceiveRpcCommandRequestComponent));
            UpdateClient();
            UpdateClient();
            Assert.AreEqual(0, ConnectingSingletonQuery.CalculateEntityCount());
        }

        [Test]
        public void WhenReceivedConnectConfirmation_ShouldDestroyRequestEntity()
        {
            State = new Connecting { State = NetworkState.GoingInGame };
            Entity entity = world.EntityManager.CreateEntity(typeof(ConfirmConnectionRequest),
                typeof(ReceiveRpcCommandRequestComponent));
            UpdateClient();
            UpdateClient();
            Assert.IsFalse(world.EntityManager.Exists(entity));
        }

        public void UpdateClient()
        {
            initBufferSystem.Update();
            world.GetExistingSystem<ClientSimulationSystemGroup>().Update();
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