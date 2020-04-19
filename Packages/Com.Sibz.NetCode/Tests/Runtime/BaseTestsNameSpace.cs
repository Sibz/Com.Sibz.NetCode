using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.TestTools;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode.PlayModeTests
{
    public class BaseTestsNameSpace
    {
        private ServerOptions serverOptions;
        private ClientOptions clientOptions;
        private ServerWorld serverWorld;
        private ClientWorld clientWorld;
        private GameObject prefab;
        private bool serverListening;
        private bool clientConnecting;
        private bool clientConnectFailed;
        private bool clientConnected;
        private ushort testCount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            prefab = Resources.Load<GameObject>("NetCodePlayModeTestCollection");
        }

        [TearDown]
        public void TearDown()
        {
            serverWorld.Dispose();
            clientWorld.Dispose();
        }

        private void NewClientServer()
        {
            ushort port = (ushort) (21650 + testCount);
            Debug.Log(port);
            serverOptions = new ServerOptions
            {
                Address = "0.0.0.0",
                Port = port,
                WorldName = $"Test_Connection_Server{testCount}",
                GhostCollectionPrefab = prefab
            };
            clientOptions = new ClientOptions
            {
                Address = "127.0.0.1",
                Port = port,
                TimeOut = 5,
                WorldName = $"Test_Connection_Client{testCount}",
                GhostCollectionPrefab = prefab
            };


            serverWorld = new ServerWorld(serverOptions);
            clientWorld = new ClientWorld(clientOptions);

            serverWorld.ListenSuccess += () =>
            {
                Debug.Log($"{serverOptions.WorldName}: Listening");
                serverListening = true;
            };
            clientWorld.Connecting += () =>
            {
                Debug.Log($"{clientOptions.WorldName}: Connecting");
                clientConnecting = true;
            };
            clientWorld.Connected += x =>
            {
                Debug.Log($"{clientOptions.WorldName}: Connected");
                clientConnected = true;
            };
            clientWorld.ConnectionFailed += x =>
            {
                Debug.Log($"{clientOptions.WorldName}: Connect Failed: " + x);
                clientConnectFailed = true;
            };
            testCount++;
        }

        [UnityTest]
        public IEnumerator ShouldConnect()
        {
            NewClientServer();
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();

            Debug.Log("Waiting for client to be connected");
            int maxCount = 60;
            while (maxCount >= 0 && !(clientConnected || clientConnectFailed))
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Debug.Log($"...Finished waiting ({(60f - maxCount) / 10f}seconds)");

            Assert.IsTrue(clientConnected);
        }

        [UnityTest]
        public IEnumerator ClientDisconnect_ShouldCallback()
        {
            bool disconnected = false;

            NewClientServer();
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();

            int maxCount = 60;
            while (maxCount >= 0 && !(clientConnected || clientConnectFailed))
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            yield return ShouldConnect();
            clientWorld.Disconnected += () => disconnected = true;
            clientWorld.Connected += e => clientWorld.Disconnect();


            maxCount = 60;
            while (maxCount >= 0 && !disconnected)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(disconnected);
        }

        [UnityTest]
        public IEnumerator Client_ShouldDisconnect()
        {
            NewClientServer();

            bool disconnected = false;

            clientWorld.Connected += e => clientWorld.Disconnect();
            clientWorld.Disconnected += () => disconnected = true;
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();

            int maxCount = 60;
            while (maxCount >= 0 && !disconnected)
            {
                yield return new WaitForSeconds(0.5f);
                maxCount--;
            }

            Assert.IsTrue(disconnected);
        }

        [UnityTest]
        public IEnumerator Server_ShouldDisconnectClient()
        {
            NewClientServer();
            bool disconnected = false;
            clientWorld.Connected += x => serverWorld.DisconnectClient(1);
            clientWorld.Disconnected += () => disconnected = true;
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();

            int maxCount = 60;
            while (maxCount >= 0 && !disconnected)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(disconnected);
        }

        [UnityTest]
        public IEnumerator ShouldConnectMultipleClients()
        {
            bool client1Connected = false, client2Connected = false;
            ClientWorld client2 = new ClientWorld(new ClientOptions
            {
                Address = "127.0.0.1",
                Port = (ushort) (21650 + testCount),
                WorldName = $"Test_Connection_Client_2_{testCount}"
            });
            NewClientServer();
            clientWorld.Connected += e => client1Connected = true;
            client2.Connected += e => client2Connected = true;
            clientWorld.Disconnected += () => client1Connected = false;
            client2.Disconnected += () => client2Connected = false;
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();
            client2.Connect();
            int maxCount = 30;
            while (maxCount >= 0 && !(client1Connected && client2Connected))
            {
                yield return new WaitForSeconds(0.25f);
                maxCount--;
            }

            Assert.IsTrue(client1Connected);
            Assert.IsTrue(client2Connected);
        }

        [UnityTest]
        public IEnumerator ShouldDisconnectAllClients()
        {
            bool client1Connected = false, client2Connected = false;
            ClientWorld client2 = new ClientWorld(new ClientOptions
            {
                Address = "127.0.0.1",
                Port = (ushort) (21650 + testCount),
                WorldName = $"Test_Connection_Client_2_{testCount}"
            });
            NewClientServer();
            clientWorld.Connected += e => client1Connected = true;
            client2.Connected += e => client2Connected = true;
            clientWorld.Disconnected += () => client1Connected = false;
            client2.Disconnected += () => client2Connected = false;
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();
            client2.Connect();
            int maxCount = 30;
            while (maxCount >= 0 && !(clientConnected && client2Connected))
            {
                yield return new WaitForSeconds(0.25f);
                maxCount--;
            }

            Assert.IsTrue(client1Connected, "Client 1 did not connect to begin test");
            Assert.IsTrue(client2Connected, "Client 2 did not connect to begin test");

            serverWorld.DisconnectAllClients();

            maxCount = 30;
            while (maxCount >= 0 && client1Connected && client2Connected)
            {
                yield return new WaitForSeconds(0.25f);
                maxCount--;
            }

            Assert.IsFalse(client1Connected, "Client 1 did not disconnect");
            Assert.IsFalse(client2Connected, "Client 2 did not disconnect");
        }

        [UnityTest]
        public IEnumerator Client_ShouldSendRpc()
        {
            NewClientServer();
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connected += e => clientWorld.World.CreateRpcRequest<PlayModeTestRequest>();
            clientWorld.Connect();

            int maxCount = 60;
            while (maxCount >= 0 && !clientConnected && !clientConnectFailed)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            /*Assert.IsNotNull(clientWorld, "cw");
            Assert.IsNotNull(clientWorld.World, "cww");
            Assert.IsNotNull(clientWorld.World.GetExistingSystem<CreateRpcRequestSystem>(), "cws");*/

            PlayModeTestRequestReceiveSystem system =
                serverWorld.World.GetExistingSystem<PlayModeTestRequestReceiveSystem>();

            maxCount = 60;
            while (maxCount >= 0 && !system.ReceivedRpc)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(system.ReceivedRpc);
        }

        [UnityTest]
        public IEnumerator Server_ShouldSendRpc()
        {
            NewClientServer();
            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connected += e => serverWorld.World.CreateRpcRequest<PlayModeTestRequest>();
            clientWorld.Connect();

            int maxCount = 60;
            while (maxCount >= 0 && !clientConnected && !clientConnectFailed)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            /*Assert.IsNotNull(clientWorld, "cw");
            Assert.IsNotNull(clientWorld.World, "cww");
            Assert.IsNotNull(clientWorld.World.GetExistingSystem<CreateRpcRequestSystem>(), "cws");*/

            PlayModeTestRequestReceiveSystem system =
                clientWorld.World.GetExistingSystem<PlayModeTestRequestReceiveSystem>();

            maxCount = 60;
            while (maxCount >= 0 && !system.ReceivedRpc)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(system.ReceivedRpc);
        }

        [UnityTest]
        public IEnumerator Server_ShouldSendRpcToMultipleClients()
        {
            ClientWorld client2 = new ClientWorld(new ClientOptions
            {
                Address = "127.0.0.1",
                Port = (ushort) (21650 + testCount),
                WorldName = $"TestServer_ShouldSendRpcToMultipleClients{testCount}"
            });
            Debug.Log((ushort) (21650 + testCount));
            NewClientServer();


            serverWorld.Listen();
            yield return new WaitForSeconds(0.1f);
            clientWorld.Connect();
            yield return new WaitForSeconds(0.1f);
            bool client2Connected = false;
            bool client2Failed = false;
            client2.Connected += e => client2Connected = true;
            client2.ConnectionFailed += s => client2Failed = true;
            client2.Connect();
            yield return new WaitForSeconds(0.1f);

            int maxCount = 90;
            while (maxCount >= 0 && !(clientConnected || clientConnectFailed) && !(client2Connected || client2Failed))
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(clientConnected, "Client 1 did not connect to begin test");
            Assert.IsTrue(client2Connected, "Client 2 did not connect to begin test");

            serverWorld.World.CreateRpcRequest<PlayModeTestRequest>(
                serverWorld.GetNetworkConnectionEntityById(clientWorld.NetworkId));
            serverWorld.World.CreateRpcRequest<PlayModeTestRequest>(
                serverWorld.GetNetworkConnectionEntityById(client2.NetworkId));

            /*Assert.IsNotNull(clientWorld, "cw");
            Assert.IsNotNull(clientWorld.World, "cww");
            Assert.IsNotNull(clientWorld.World.GetExistingSystem<CreateRpcRequestSystem>(), "cws");*/

            PlayModeTestRequestReceiveSystem system1 =
                clientWorld.World.GetExistingSystem<PlayModeTestRequestReceiveSystem>();
            PlayModeTestRequestReceiveSystem system2 =
                client2.World.GetExistingSystem<PlayModeTestRequestReceiveSystem>();

            maxCount = 60;
            while (maxCount >= 0 && !system1.ReceivedRpc && !system2.ReceivedRpc)
            {
                yield return new WaitForSeconds(0.1f);
                maxCount--;
            }

            Assert.IsTrue(system1.ReceivedRpc && system2.ReceivedRpc);
        }
    }

    [ClientAndServerSystem]
    public class PlayModeTestRequestSystem : RpcCommandRequestSystem<PlayModeTestRequest>
    {
    }

    [ClientAndServerSystem]
    public class PlayModeTestRequestReceiveSystem : SystemBase
    {
        public bool ReceivedRpc;
        private EntityQuery eq;

        protected override void OnCreate()
        {
            eq = GetEntityQuery(typeof(PlayModeTestRequest), typeof(ReceiveRpcCommandRequestComponent));
            RequireForUpdate(eq);
        }

        protected override void OnUpdate()
        {
            ReceivedRpc = true;
        }
    }
}