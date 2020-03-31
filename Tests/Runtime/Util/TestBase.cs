using System.Collections;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.TestTools;

namespace Sibz.NetCode.Tests
{
    public class TestBase
    {
        protected World World;

        /*[OneTimeSetUp]
        public void OneTimeSetup()
        {
            //DefaultWorldInitialization.Initialize("DefaultWorldSomething", true);
            //yield return null;

            DefaultWorldInitialization.Initialize("DefaultWorld", true);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(World.DefaultGameObjectInjectionWorld,
                DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default));
            new ClientServerBootstrap().Initialize("DefaultWorld");
        }*/

        [UnitySetUp]
        public virtual IEnumerator SetUp()
        {
            World = new World("Test");

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            World.Dispose();
        }

        /*[OneTimeTearDown]
        public void TD()
        {
            for (int i = World.All.Count - 1; i >= 0; i--)
            {
                if (World.All[i].Name == "Test" || World.All[i].Name == "DefaultWorldSomething"
                                                || World.All[i].Name.StartsWith("Client")
                                                || World.All[i].Name.StartsWith("Server"))
                    World.All[i].Dispose();
            }
        }*/


    }

    /*public class TestBase
    {
        protected World World;


        [SetUp]
        public void SetUp()
        {
            World = new World("Test");
            //DefaultWorldInitialization.Initialize("DefaultWorld", true);
            //yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            World.Dispose();
        }

        protected static string MakeTestWorldName()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            return $"Test_{methodBase.Name}";
        }
    }*/
}