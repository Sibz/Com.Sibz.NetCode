using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            yield return new WaitForSeconds(0.5f);
            clientWorld.Connect();

            int maxCount = 120;
            while (maxCount >= 0 && !clientConnected && !clientConnectFailed)
            {
                yield return new WaitForSeconds(0.05f);
                maxCount--;
            }

            Assert.IsTrue(clientConnected);
        }

        [UnityTest]
        public IEnumerator ClientDisconnect_ShouldCallback()
        {
            bool disconnected = false;

            yield return ShouldConnect();
            clientWorld.Disconnected += () => disconnected = true;

            clientWorld.Disconnect();

            int maxCount = 60;
            while (maxCount >= 0 && !disconnected)
            {
                yield return new WaitForSeconds(0.05f);
                maxCount--;
            }

            Assert.IsTrue(disconnected);
        }

        [UnityTest]
        public IEnumerator ShouldDisconnectClient()
        {
            NewClientServer();
            bool disconnected = false;
            clientWorld.Connected += x => serverWorld.DisconnectClient(1);
            clientWorld.Disconnected += () => disconnected = true;
            serverWorld.Listen();
            yield return new WaitForSeconds(0.5f);
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
            yield return new WaitForSeconds(0.5f);
            clientWorld.Connect();
            client2.Connect();
            int maxCount = 30;
            while (maxCount >= 0 && !(client1Connected && client2Connected))
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
    }
}