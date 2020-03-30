using System;
using System.Linq;
using System.Reflection;

namespace Sibz.NetCode.Tests.Util
{
    public class TestHelpers
    {
        public static T GetInstanceOf<T>(string className, string nameSpace = null)
            where T : class =>
            Activator.CreateInstance(Assembly.GetAssembly(typeof(T)).GetTypes()
                .First(x => x.Name == className && x.IsClass && !(x.GetInterface(typeof(T).Name) is null) &&
                            (nameSpace is null || x.Namespace == nameSpace))) as T;
    }
}