using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace Sibz.NetCode.Tests
{
    public class TestBase
    {
        protected World World;

        [SetUp]
        public void SetUp()
        {
            World = new World("Test");
        }

        [TearDown]
        public void TearDown()
        {
            World.Dispose();
        }
    }
}