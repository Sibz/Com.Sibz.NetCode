﻿using Unity.Entities;
using Unity.NetCode;

namespace Sibz.NetCode
{
    public struct ConvertToRpcRequestJob
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public Entity TargetConnection;

        public void Execute(Entity e, int i)
        {
            CommandBuffer.AddComponent(i, e,
                new SendRpcCommandRequestComponent {TargetConnection = TargetConnection});
        }
    }
}