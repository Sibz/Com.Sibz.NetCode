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
        private ServerWorld serverWorld;
        private ClientWorld clientWorld;
        private bool serverListening = false;
        private bool clientConnecting = false;
        private bool clientConnectFailed = false;
        private bool clientConnected = false;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [UnityTest]
        public IEnumerator ShouldConnect()
        {
            List<GameObject> prefabs = new List<GameObject>();
                //{ Resources.Load<GameObject>("NetCodePlayModeTestCollection") };

            ServerOptions serverOptions = new ServerOptions
            {
                Address = "0.0.0.0",
                Port = 21650,
                WorldName = "Test_Connection_Server",
                GhostCollectionPrefabs =prefabs,
            };
            ClientOptions clientOptions = new ClientOptions
            {
                Address = "127.0.0.1",
                Port = 21650,
                TimeOut = 2,
                WorldName = "Test_Connection_Client",
                GhostCollectionPrefabs = prefabs
            };
            serverWorld = new ServerWorld(serverOptions);
            clientWorld = new ClientWorld(clientOptions);

            serverWorld.ListenSuccess += () =>
            {
                Debug.Log("Listening");
                serverListening = true;
            };
            clientWorld.Connecting += () =>
            {
                Debug.Log("Connecting");
                clientConnecting = true;
            };
            clientWorld.Connected += x =>
            {
                Debug.Log("Connected");
                clientConnected = true;
            };
            clientWorld.ConnectionFailed += x =>
            {
                Debug.Log("Connect Failed: " + x);
                clientConnectFailed = true;
            };


            serverWorld.Listen();
            //UpdateWorlds();
            clientWorld.Connect();
            //UpdateWorlds();

            int maxCount = 120;
            while (maxCount >= 0 && !clientConnected && !clientConnectFailed)
            {
                yield return new WaitForSeconds(0.05f);
                //UpdateWorlds();
                maxCount--;
            }
            Assert.IsTrue(clientConnected);
        }

        /*private void UpdateWorlds()
        {
            serverWorld.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            serverWorld.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }*/

    }
}