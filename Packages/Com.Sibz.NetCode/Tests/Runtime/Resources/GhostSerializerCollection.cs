using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

public struct NetCodeGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        string[] arr = new[]
        {
            "NetCodePlayModeTestGhostObjectGhostSerializer"
        };
        return arr;
    }

    public int Length => 1;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(NetCodePlayModeTestGhostObjectSnapshotData))
        {
            return 0;
        }

        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_NetCodePlayModeTestGhostObjectGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_NetCodePlayModeTestGhostObjectGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_NetCodePlayModeTestGhostObjectGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<NetCodeGhostSerializerCollection>
                    .InvokeSerialize<NetCodePlayModeTestGhostObjectGhostSerializer,
                        NetCodePlayModeTestGhostObjectSnapshotData>(m_NetCodePlayModeTestGhostObjectGhostSerializer,
                        ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private NetCodePlayModeTestGhostObjectGhostSerializer m_NetCodePlayModeTestGhostObjectGhostSerializer;
}

public struct EnableNetCodeGhostSendSystemComponent : IComponentData
{
}

public class NetCodeGhostSendSystem : GhostSendSystem<NetCodeGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableNetCodeGhostSendSystemComponent>();
    }
}