using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct NetCodeGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "NetCodePlayModeTestGhostObjectGhostSerializer",
        };
        return arr;
    }

    public int Length => 1;
#endif
    public void Initialize(World world)
    {
        var curNetCodePlayModeTestGhostObjectGhostSpawnSystem = world.GetOrCreateSystem<NetCodePlayModeTestGhostObjectGhostSpawnSystem>();
        m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhostIds = curNetCodePlayModeTestGhostObjectGhostSpawnSystem.NewGhostIds;
        m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhosts = curNetCodePlayModeTestGhostObjectGhostSpawnSystem.NewGhosts;
        curNetCodePlayModeTestGhostObjectGhostSpawnSystem.GhostType = 0;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_NetCodePlayModeTestGhostObjectSnapshotDataFromEntity = system.GetBufferFromEntity<NetCodePlayModeTestGhostObjectSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<NetCodeGhostDeserializerCollection>.InvokeDeserialize(m_NetCodePlayModeTestGhostObjectSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhostIds.Add(ghostId);
                m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhosts.Add(GhostReceiveSystem<NetCodeGhostDeserializerCollection>.InvokeSpawn<NetCodePlayModeTestGhostObjectSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<NetCodePlayModeTestGhostObjectSnapshotData> m_NetCodePlayModeTestGhostObjectSnapshotDataFromEntity;
    private NativeList<int> m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhostIds;
    private NativeList<NetCodePlayModeTestGhostObjectSnapshotData> m_NetCodePlayModeTestGhostObjectSnapshotDataNewGhosts;
}
public struct EnableNetCodeGhostReceiveSystemComponent : IComponentData
{}
public class NetCodeGhostReceiveSystem : GhostReceiveSystem<NetCodeGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableNetCodeGhostReceiveSystemComponent>();
    }
}
