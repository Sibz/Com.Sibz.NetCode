using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.TestTools;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode.Tests
{
    public class BaseTestsNameSpace
    {
        /*private ServerWorld serverWorld;
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
            List<GameObject> prefabs = new List<GameObject>
                { Resources.Load<GameObject>("NetCodeTestCollection") };

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
                TimeOut = 10,
                WorldName = "Test_Connection_Client",
                GhostCollectionPrefabs = prefabs
            };
            serverWorld = new ServerWorld(serverOptions);
            clientWorld = new ClientWorld(clientOptions);

            serverWorld.ListenSuccess += () => serverListening = true;
            clientWorld.Connecting += () => clientConnecting = true;
            clientWorld.Connected += x => clientConnected = true;
            clientWorld.ConnectionFailed += x => clientConnectFailed = true;


            serverWorld.Listen();
            UpdateWorlds();
            clientWorld.Connect();
            UpdateWorlds();

            int maxCount = 120;
            while (maxCount >= 0 && !clientConnected && !clientConnectFailed)
            {
                yield return null;
                UpdateWorlds();
                maxCount--;
            }
            Assert.IsTrue(clientConnected);
        }

        private void UpdateWorlds()
        {
            serverWorld.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientInitializationSystemGroup>().Update();
            serverWorld.World.GetExistingSystem<ServerInitializationSystemGroup>().Update();
            clientWorld.World.GetExistingSystem<ClientSimulationSystemGroup>().Update();
        }*/

    }
}