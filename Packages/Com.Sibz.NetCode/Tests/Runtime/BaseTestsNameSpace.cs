using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
/*using Unity.Entities;
using Unity.NetCode;*/
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
        private List<GameObject> prefabs;
        private bool serverListening = false;
        private bool clientConnecting = false;
        private bool clientConnectFailed = false;
        private bool clientConnected = false;
        private ushort testCount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            prefabs = new List<GameObject>
            {
                Resources.Load<GameObject>("NetCodePlayModeTestCollection")
            };
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
                GhostCollectionPrefabs = prefabs,
            };
            clientOptions = new ClientOptions
            {
                Address = "127.0.0.1",
                Port = port,
                TimeOut = 5,
                WorldName = $"Test_Connection_Client{testCount}",
                GhostCollectionPrefabs = prefabs
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
            clientWorld.Connected += (x)=> serverWorld.DisconnectClient(1);
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
    }
}