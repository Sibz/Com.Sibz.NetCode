using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct NetCodeGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "NetCodeTestGhostObjectGhostSerializer",
        };
        return arr;
    }

    public int Length => 1;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(NetCodeTestGhostObjectSnapshotData))
            return 0;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_NetCodeTestGhostObjectGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_NetCodeTestGhostObjectGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_NetCodeTestGhostObjectGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<NetCodeGhostSerializerCollection>.InvokeSerialize<NetCodeTestGhostObjectGhostSerializer, NetCodeTestGhostObjectSnapshotData>(m_NetCodeTestGhostObjectGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private NetCodeTestGhostObjectGhostSerializer m_NetCodeTestGhostObjectGhostSerializer;
}

public struct EnableNetCodeGhostSendSystemComponent : IComponentData
{}
public class NetCodeGhostSendSystem : GhostSendSystem<NetCodeGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableNetCodeGhostSendSystemComponent>();
    }

    public override bool IsEnabled()
    {
        return HasSingleton<EnableNetCodeGhostSendSystemComponent>();
    }
}
