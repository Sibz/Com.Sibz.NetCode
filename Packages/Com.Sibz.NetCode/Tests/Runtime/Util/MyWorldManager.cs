using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests
{
    public class MyWorldManager : WorldManagerBase<ClientSimulationSystemGroup>
    {
        public bool CalledBootStrapCreateWorld;
        public bool CalledImportPrefabs;

        protected override World BootStrapCreateWorld(string name)
        {
            CalledBootStrapCreateWorld = true;
            return ClientServerBootstrap.CreateClientWorld(NetCodeFixture.DefaultWorld, name);
        }

        protected override void ImportPrefabs() => CalledImportPrefabs = true;

        public MyWorldManager(IWorldManagerOptions options) : base(options)
        {
        }
    }
}