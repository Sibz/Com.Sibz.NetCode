using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sibz
{
    public static partial class Util
    {
        public static List<Type> AppendTypesWithAttribute(this List<Type> types, Type type)
        {
            types = types ?? new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                types.AddRange(a.GetTypes().Where(x => !(x.GetCustomAttribute(type) is null)));
            }

            return types;
        }
        public static List<Type> AppendTypesWithAttribute<T>(this List<Type> types)
            where T : Attribute
        {
            return AppendTypesWithAttribute(types, typeof(T));
        }
    }
}