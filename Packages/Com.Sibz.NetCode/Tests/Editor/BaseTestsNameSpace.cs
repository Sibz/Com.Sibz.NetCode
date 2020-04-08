using NUnit.Framework;
using Unity.Entities;

[assembly: DisableAutoCreation]

namespace Sibz.NetCode.Tests
{
    public class BaseTestsNameSpace
    {
        [Test]
        public void OneTest()
        {
            Assert.IsTrue(true);
        }
    }
}