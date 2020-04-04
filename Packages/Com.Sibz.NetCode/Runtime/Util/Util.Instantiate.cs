using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz
{
    public static partial class Util
    {
        public static void Instantiate(this IEnumerable<GameObject> list)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            GameObject[] prefabs = list.ToArray();
            foreach (GameObject prefab in prefabs)
            {
                Object.Instantiate(prefab);
            }
        }
    }
}