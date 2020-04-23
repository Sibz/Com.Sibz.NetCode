using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;

namespace Sibz
{
    public static partial class Util
    {
        private static readonly AssCache AssemblyCache = new AssCache();
        private class AssCache
        {
            private Assembly[] assemblies;
            private Type[] classTypes;
            private Type[] classesWithAttributeTypes;
            private Type[] classesWithBaseTypes;
            private readonly Dictionary<Type, Type[]> classesWithAttribute = new Dictionary<Type, Type[]>();

            public Assembly[] Assemblies => assemblies ?? (assemblies = AppDomain.CurrentDomain.GetAssemblies());

            public Type[] ClassTypes
            {
                get
                {
                    if (!(classTypes is null))
                    {
                        return classTypes;
                    }

                    List<Type> types = new List<Type>();
                    foreach (Assembly a in Assemblies)
                    {
                        types.AddRange(a.GetTypes().Where(x=>x.IsClass && x.IsPublic));
                    }

                    classTypes = types.ToArray();

                    return classTypes;
                }
            }

            public Type[] ClassesWithAttributeTypes
                => classesWithAttributeTypes ?? (classesWithAttributeTypes =
                    ClassTypes.Where(x => x.CustomAttributes.Any()).ToArray());

            public Type[] ClassesWithBaseTypes => classesWithBaseTypes ??
                                                  (classesWithBaseTypes = ClassTypes.Where(x => !(x.BaseType is null))
                                                      .ToArray());

            public Type[] this[Type index]
            {
                get
                {
                    if (classesWithAttribute.ContainsKey(index))
                    {
                        return classesWithAttribute[index];
                    }

                    List<Type> types = new List<Type>();
                    for (int i = 0; i < ClassesWithAttributeTypes.Length; i++)
                    {
                        if (!(ClassesWithAttributeTypes[i].GetCustomAttribute(index) is null))
                        {
                            types.Add(ClassesWithAttributeTypes[i]);
                        }
                    }
                    classesWithAttribute.Add(index, types.ToArray());

                    return classesWithAttribute[index];
                }
            }
        }

        public static List<Type> AppendTypesWithAttribute(this List<Type> types, Type type)
        {
            types = types ?? new List<Type>();

            types.AddRange(AssemblyCache[type]);

            return types;
        }

        public static List<Type> AppendTypesWithUpdateInGroupAttribute(this List<Type> types, Type groupType)
        {
            types = types ?? new List<Type>();

            Type[] updateInGroupClassTypes = AssemblyCache[typeof(UpdateInGroupAttribute)];
            for (int i = 0; i < updateInGroupClassTypes.Length; i++)
            {
                if (updateInGroupClassTypes[i].GetCustomAttribute<UpdateInGroupAttribute>().GroupType == groupType)
                {
                    types.Add(groupType);
                }
            }
            return types;
        }

        public static List<Type> AppendTypesThatAreSubclassOf(this List<Type> types, string baseTypeName)
        {
            types = types ?? new List<Type>();

            for (int i = 0; i < AssemblyCache.ClassesWithBaseTypes.Length; i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (AssemblyCache.ClassesWithBaseTypes[i].BaseType.Name.StartsWith(baseTypeName))
                {
                    types.Add(AssemblyCache.ClassesWithBaseTypes[i]);
                }
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