using System.Collections;
using NUnit.Framework;
using Unity.Entities;
using UnityEngine.TestTools;

namespace Sibz.NetCode.Tests
{
    public class TestBase
    {
        protected World World;

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
    }
}