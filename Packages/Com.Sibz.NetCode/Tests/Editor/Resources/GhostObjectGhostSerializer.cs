using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public struct NetCodeTestGhostObjectGhostSerializer : IGhostSerializer<NetCodeTestGhostObjectSnapshotData>
{
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;

    private ComponentType componentTypeTranslation;

    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction] [ReadOnly]
    private ArchetypeChunkComponentType<Rotation> ghostRotationType;

    [NativeDisableContainerSafetyRestriction] [ReadOnly]
    private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<NetCodeTestGhostObjectSnapshotData>();

    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick,
        ref NetCodeTestGhostObjectSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        NativeArray<Rotation> chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        NativeArray<Translation> chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}