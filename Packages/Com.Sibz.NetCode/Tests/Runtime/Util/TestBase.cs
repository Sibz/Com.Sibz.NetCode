using NUnit.Framework;
using Unity.Entities;

namespace Sibz.NetCode.Tests
{
    public class TestBase
    {
        protected World World;

        [SetUp]
        public void SetUp() => World = new World("Test");

        [TearDown]
        public void TearDown() => World.Dispose();
    }
}