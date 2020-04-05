using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

public struct NetCodeGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new[]
        {
            "NetCodeTestGhostObjectGhostSerializer"
        };
        return arr;
    }

    public int Length => 1;
#endif
    public void Initialize(World world)
    {
        var curNetCodeTestGhostObjectGhostSpawnSystem =
            world.GetOrCreateSystem<NetCodeTestGhostObjectGhostSpawnSystem>();
        m_NetCodeTestGhostObjectSnapshotDataNewGhostIds = curNetCodeTestGhostObjectGhostSpawnSystem.NewGhostIds;
        m_NetCodeTestGhostObjectSnapshotDataNewGhosts = curNetCodeTestGhostObjectGhostSpawnSystem.NewGhosts;
        curNetCodeTestGhostObjectGhostSpawnSystem.GhostType = 0;
    }

    public void BeginDeserialize(JobComponentSystem system) => m_NetCodeTestGhostObjectSnapshotDataFromEntity =
        system.GetBufferFromEntity<NetCodeTestGhostObjectSnapshotData>();

    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<NetCodeGhostDeserializerCollection>.InvokeDeserialize(
                    m_NetCodeTestGhostObjectSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
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
                m_NetCodeTestGhostObjectSnapshotDataNewGhostIds.Add(ghostId);
                m_NetCodeTestGhostObjectSnapshotDataNewGhosts.Add(
                    GhostReceiveSystem<NetCodeGhostDeserializerCollection>
                        .InvokeSpawn<NetCodeTestGhostObjectSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<NetCodeTestGhostObjectSnapshotData> m_NetCodeTestGhostObjectSnapshotDataFromEntity;
    private NativeList<int> m_NetCodeTestGhostObjectSnapshotDataNewGhostIds;
    private NativeList<NetCodeTestGhostObjectSnapshotData> m_NetCodeTestGhostObjectSnapshotDataNewGhosts;
}

public struct EnableNetCodeGhostReceiveSystemComponent : IComponentData
{
}

public class NetCodeGhostReceiveSystem : GhostReceiveSystem<NetCodeGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableNetCodeGhostReceiveSystemComponent>();
    }
}