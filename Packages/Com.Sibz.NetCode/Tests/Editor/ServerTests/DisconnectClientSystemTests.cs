using NUnit.Framework;
using Packages.Com.Sibz.NetCode.Server.Runtime.Systems;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectClientSystemTests
    {
        private World world;
        private MyDisconnectClientSystem system;
        private int testCount;

        [SetUp]
        public void SetUp()
        {
            world = new World($"TestDisconnectClientSystem{testCount++}");
            world.CreateSystem<DisconnectClientSystem>();
        }


        public class MyDisconnectClientSystem : DisconnectClientSystem
        {
            public bool DidUpdate;

            protected override void OnUpdate()
            {
                DidUpdate = true;
                base.OnUpdate();
            }
        }
    }
}