﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Sibz.EntityEvents;
using Unity.Collections;
using Unity.Entities;

namespace Sibz.NetCode
{

    [ClientAndServerSystem]
    public class HookSystem : ComponentSystem
    {
        private EntityQuery allEventComponentsQuery;

        protected readonly Dictionary<ComponentType, Action<IEventComponentData>> ActionMap =
            new Dictionary<ComponentType, Action<IEventComponentData>>();

        private MethodInfo getComponentMethodInfo;

        public void RegisterHook<T>(Action<IEventComponentData> action)
            where T : struct, IComponentData =>
            RegisterHook(typeof(T), action);

        public void RegisterHook(ComponentType componentType, Action<IEventComponentData> action)
        {
            if (ActionMap.ContainsKey(componentType))
            {
                ActionMap[componentType] = action;
                return;
            }

            ActionMap.Add(componentType, action);
        }

        public void RegisterHooks(Dictionary<ComponentType, Action<IEventComponentData>> types)
        {
            foreach (var type in types)
            {
                RegisterHook(type.Key, type.Value);
            }
        }

        public void UnregisterHook<T>()
            where T : struct, IEventComponentData => UnregisterHook(typeof(T));

        public void UnregisterHook(ComponentType componentType)
        {
            if (ActionMap.ContainsKey(componentType))
            {
                ActionMap.Remove(componentType);
            }
        }

        private void ExecuteHook<TEvent>(IEventComponentData eventData)
            where TEvent : struct, IEventComponentData
        {
            if (ActionMap.ContainsKey(typeof(TEvent)))
            {
                ActionMap[typeof(TEvent)].Invoke(eventData);
            }
        }

        protected override void OnCreate()
        {
            allEventComponentsQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    Any = EventComponentSystem.EventTypes
                });
            RequireForUpdate(allEventComponentsQuery);

            getComponentMethodInfo = EntityManager
                .GetType()
                .GetMethod(nameof(EntityManager.GetComponentData));
        }

        protected override void OnUpdate()
        {
            Entities.With(allEventComponentsQuery).ForEach((e) =>
            {
                var components = EntityManager.GetComponentTypes(e, Allocator.TempJob);
                if (components.Length == 0)
                {
                    return;
                }

                if (!ActionMap.ContainsKey(components[0]))
                {
                    return;
                }

                if (components[0].IsZeroSized)
                {
                    ActionMap[components[0]].Invoke(default);
                    return;
                }

                Type type = components[0].GetManagedType();
                MethodInfo getComponent = getComponentMethodInfo.MakeGenericMethod(type);
                ActionMap[components[0]]
                    .Invoke(getComponent.Invoke(EntityManager, new object[] {e}) as IEventComponentData);
            });
        }
    }
}