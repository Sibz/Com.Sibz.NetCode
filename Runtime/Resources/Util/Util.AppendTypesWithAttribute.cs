using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sibz
{
    public static partial class Util
    {
        public static List<Type> AppendTypesWithAttribute<T>(this List<Type> types)
        where T: Attribute
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                types.AddRange(a.GetTypes().Where(x => !(x.GetCustomAttribute<T>() is null)));
            }

            return types;
        }
    }
}