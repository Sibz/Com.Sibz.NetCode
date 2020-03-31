using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.NetCode.Internal
{
    public static class Util
    {
        public static void InstantiateFromList(IEnumerable<GameObject> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            var prefabs = list.ToArray();
            foreach (GameObject prefab in prefabs)
            {
                Object.Instantiate(prefab);
            }
        }

        public static List<Type> GetSystemsWithAttribute<T>(List<Type> systemTypes = null)
        where T:Attribute
        {
            systemTypes = systemTypes ?? new List<Type>();
            systemTypes.AddRange(Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(x => !(x.GetCustomAttribute<T>() is null)));
            return systemTypes;
        }
    }
}