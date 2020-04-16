using Packages.Com.Sibz.NetCode.Server.Runtime.Systems;

namespace Sibz.NetCode.Tests.Server
{
    public class DisconnectClientSystemTests
    {
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