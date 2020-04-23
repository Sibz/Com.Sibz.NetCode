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
            private readonly Dictionary<Type, Type[]> classesWithSpecificUpdateGroup = new Dictionary<Type, Type[]>();
            private readonly Dictionary<string, Type[]> classesThatBaseClassStartsWith = new Dictionary<string, Type[]>();

            private static readonly string[] ExcludedAssemblies =
            {
                "mscorlib,",
                "Accessibility,",
                "Unity.",
                "UnityEngine,",
                "UnityEngine.",
                "UnityEditor,",
                "UnityEditor.",
                "System,",
                "System.",
                "nunit.framework,",
                "ReportGeneratorMerged,",
                "netstandard",
                "ExCSS.Unity,",
                "JetBrains.",
                "Mono.",
                "Novell.",
                "Microsoft."
            };

            public Assembly[] Assemblies
            {
                get
                {
                    if (!(assemblies is null))
                        return assemblies;

                    List<Assembly> asses = new List<Assembly>();
                    var localAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < localAssemblies.Length; i++)
                    {
                        bool exclude = false;
                        for (int j = 0; j < ExcludedAssemblies.Length; j++)
                        {
                            if (!localAssemblies[i].FullName.StartsWith(ExcludedAssemblies[j]))
                            {
                                continue;
                            }

                            exclude = true;
                            break;
                        }

                        if (!exclude)
                        {
                            asses.Add(localAssemblies[i]);
                        }

                    }

                    assemblies = asses.ToArray();
                    return assemblies;
                }
            }



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
                        types.AddRange(a.GetTypes().Where(x => x.IsClass && x.IsPublic));
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
                        foreach (CustomAttributeData customAttributeData in ClassesWithAttributeTypes[i].CustomAttributes)
                        {
                            if (customAttributeData.AttributeType != index)
                            {
                                continue;
                            }

                            types.Add(ClassesWithAttributeTypes[i]);
                            break;
                        }
                    }

                    classesWithAttribute.Add(index, types.ToArray());

                    return classesWithAttribute[index];
                }
            }

            public Type[] GetClassesWithSpecificUpdateGroup(Type updateGroup)
            {
                if (classesWithSpecificUpdateGroup.ContainsKey(updateGroup))
                {
                    return classesWithSpecificUpdateGroup[updateGroup];
                }

                List<Type> types = new List<Type>();
                Type[] updateInGroupClassTypes = AssemblyCache[typeof(UpdateInGroupAttribute)];
                for (int i = 0; i < updateInGroupClassTypes.Length; i++)
                {
                    if (updateInGroupClassTypes[i].GetCustomAttribute<UpdateInGroupAttribute>().GroupType == updateGroup)
                    {
                        types.Add(updateInGroupClassTypes[i]);
                    }
                }
                classesWithSpecificUpdateGroup.Add(updateGroup, types.ToArray());
                return classesWithSpecificUpdateGroup[updateGroup];
            }

            public Type[] GetClassesTheBaseTypesNameStartsWith(string name)
            {
                if (classesThatBaseClassStartsWith.ContainsKey(name))
                {
                    return classesThatBaseClassStartsWith[name];
                }
                List<Type> types = new List<Type>();
                for (int i = 0; i < ClassesWithBaseTypes.Length; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (ClassesWithBaseTypes[i].BaseType.Name.StartsWith(name))
                    {
                        types.Add(ClassesWithBaseTypes[i]);
                    }
                }
                classesThatBaseClassStartsWith.Add(name, types.ToArray());
                return classesThatBaseClassStartsWith[name];
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

            types.AddRange(AssemblyCache.GetClassesWithSpecificUpdateGroup(groupType));

            return types;
        }

        public static List<Type> AppendTypesThatAreSubclassOf(this List<Type> types, string baseTypeName)
        {
            types = types ?? new List<Type>();

            types.AddRange(AssemblyCache.GetClassesTheBaseTypesNameStartsWith(baseTypeName));

            return types;
        }

        public static List<Type> AppendTypesWithAttribute<T>(this List<Type> types)
            where T : Attribute
        {
            return AppendTypesWithAttribute(types, typeof(T));
        }
    }
}