using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Unity.Entities;

namespace Sibz.NetCode.Tests.Util
{
    public class TestBase
    {
        protected World World;

        [SetUp]
        public void SetUp()
        {
            World = new World("Test");
            DefaultWorldInitialization.Initialize("DefaultWorld", false);
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
    }
}