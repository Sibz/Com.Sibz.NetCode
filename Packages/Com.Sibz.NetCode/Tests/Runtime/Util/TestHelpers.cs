using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sibz.NetCode.Tests
{
    public class TestUtil
    {
        public static T GetInstanceOf<T>(string className, string nameSpace = null)
            where T : class
        {
            return Activator.CreateInstance(Assembly.GetAssembly(typeof(T)).GetTypes()
                .First(x => x.Name == className && x.IsClass && !(x.GetInterface(typeof(T).Name) is null) &&
                            (nameSpace is null || x.Namespace == nameSpace))) as T;
        }

        protected static string MakeTestWorldName()
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            return $"Test_{methodBase.Name}";
        }
    }
}