using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;

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

        public static List<Type> AppendTypesWithUpdateInGroupAttribute(this List<Type> types, Type groupType)
        {
            types = types ?? new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                types.AddRange(a.GetTypes().Where(x =>
                    x.GetCustomAttribute<UpdateInGroupAttribute>() is UpdateInGroupAttribute att
                    && att.GroupType == groupType));
            }

            return types;
        }
        public static List<Type> AppendTypesThatAreSubclassOf(this List<Type> types, string baseTypeName)
        {
            types = types ?? new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                types.AddRange(a.GetTypes().Where(x =>
                    x.BaseType != null
                    && x.BaseType.Name.StartsWith(baseTypeName)));
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