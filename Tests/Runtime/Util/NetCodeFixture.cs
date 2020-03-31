using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode.Tests
{
    [SetUpFixture]
    public class NetCodeFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            DefaultWorldInitialization.Initialize("DefaultWorld", true);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(World.DefaultGameObjectInjectionWorld,
                DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default));
            new ClientServerBootstrap().Initialize("DefaultWorld");
        }

        [OneTimeTearDown]
        public void TD()
        {
            for (int i = World.All.Count - 1; i >= 0; i--)
            {
                if (World.All[i].Name == "Test" || World.All[i].Name == "DefaultWorldSomething"
                                                || World.All[i].Name.StartsWith("Client")
                                                || World.All[i].Name.StartsWith("Server"))
                    World.All[i].Dispose();
            }
        }
    }
}