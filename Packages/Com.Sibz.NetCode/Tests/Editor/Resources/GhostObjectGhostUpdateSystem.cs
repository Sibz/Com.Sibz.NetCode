using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.NetCode;
using Unity.Networking.Transport.Utilities;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
public class NetCodeTestGhostObjectGhostUpdateSystem : JobComponentSystem
{
    private ClientSimulationSystemGroup m_ClientSimulationSystemGroup;
    private GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    private EntityQuery m_interpolatedQuery;
    private EntityQuery m_predictedQuery;
    private NativeHashMap<int, GhostEntity> m_ghostEntityMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private NativeArray<uint> m_ghostMinMaxSnapshotTick;
#endif
    private GhostUpdateSystemGroup m_GhostUpdateSystemGroup;
    private uint m_LastPredictedTick;

    protected override void OnCreate()
    {
        m_GhostUpdateSystemGroup = World.GetOrCreateSystem<GhostUpdateSystemGroup>();
        m_ghostEntityMap = m_GhostUpdateSystemGroup.GhostEntityMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        m_ghostMinMaxSnapshotTick = m_GhostUpdateSystemGroup.GhostSnapshotTickMinMax;
#endif
        m_ClientSimulationSystemGroup = World.GetOrCreateSystem<ClientSimulationSystemGroup>();
        m_GhostPredictionSystemGroup = World.GetOrCreateSystem<GhostPredictionSystemGroup>();
        m_interpolatedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadWrite<NetCodeTestGhostObjectSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>()
            },
            None = new[] { ComponentType.ReadWrite<PredictedGhostComponent>() }
        });
        m_predictedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<NetCodeTestGhostObjectSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadOnly<PredictedGhostComponent>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>()
            }
        });
        RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<NetCodeTestGhostObjectSnapshotData>(),
            ComponentType.ReadOnly<GhostComponent>()));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!m_predictedQuery.IsEmptyIgnoreFilter)
        {
            UpdatePredictedJob updatePredictedJob = new UpdatePredictedJob
            {
                GhostMap = m_ghostEntityMap,
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                minMaxSnapshotTick = m_ghostMinMaxSnapshotTick,
#endif
                minPredictedTick = m_GhostPredictionSystemGroup.OldestPredictedTick,
                ghostSnapshotDataType = GetArchetypeChunkBufferType<NetCodeTestGhostObjectSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                predictedGhostComponentType = GetArchetypeChunkComponentType<PredictedGhostComponent>(),
                ghostRotationType = GetArchetypeChunkComponentType<Rotation>(),
                ghostTranslationType = GetArchetypeChunkComponentType<Translation>(),

                targetTick = m_ClientSimulationSystemGroup.ServerTick,
                lastPredictedTick = m_LastPredictedTick
            };
            m_LastPredictedTick = m_ClientSimulationSystemGroup.ServerTick;
            if (m_ClientSimulationSystemGroup.ServerTickFraction < 1)
            {
                m_LastPredictedTick = 0;
            }

            inputDeps = updatePredictedJob.Schedule(m_predictedQuery,
                JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
            m_GhostPredictionSystemGroup.AddPredictedTickWriter(inputDeps);
        }

        if (!m_interpolatedQuery.IsEmptyIgnoreFilter)
        {
            UpdateInterpolatedJob updateInterpolatedJob = new UpdateInterpolatedJob
            {
                GhostMap = m_ghostEntityMap,
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                minMaxSnapshotTick = m_ghostMinMaxSnapshotTick,
#endif
                ghostSnapshotDataType = GetArchetypeChunkBufferType<NetCodeTestGhostObjectSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                ghostRotationType = GetArchetypeChunkComponentType<Rotation>(),
                ghostTranslationType = GetArchetypeChunkComponentType<Translation>(),
                targetTick = m_ClientSimulationSystemGroup.InterpolationTick,
                targetTickFraction = m_ClientSimulationSystemGroup.InterpolationTickFraction
            };
            inputDeps = updateInterpolatedJob.Schedule(m_interpolatedQuery,
                JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
        }

        return inputDeps;
    }

    [BurstCompile]
    private struct UpdateInterpolatedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<uint> minMaxSnapshotTick;
#pragma warning disable 649
        [NativeSetThreadIndex] public int ThreadIndex;
#pragma warning restore 649
#endif
        [ReadOnly] public ArchetypeChunkBufferType<NetCodeTestGhostObjectSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<Rotation> ghostRotationType;
        public ArchetypeChunkComponentType<Translation> ghostTranslationType;

        public uint targetTick;
        public float targetTickFraction;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            GhostDeserializerState deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            NativeArray<Entity> ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            BufferAccessor<NetCodeTestGhostObjectSnapshotData> ghostSnapshotDataArray =
                chunk.GetBufferAccessor(ghostSnapshotDataType);
            NativeArray<Rotation> ghostRotationArray = chunk.GetNativeArray(ghostRotationType);
            NativeArray<Translation> ghostTranslationArray = chunk.GetNativeArray(ghostTranslationType);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            int minMaxOffset = ThreadIndex * (JobsUtility.CacheLineSize / 4);
#endif
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                DynamicBuffer<NetCodeTestGhostObjectSnapshotData> snapshot = ghostSnapshotDataArray[entityIndex];
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                uint latestTick = snapshot.GetLatestTick();
                if (latestTick != 0)
                {
                    if (minMaxSnapshotTick[minMaxOffset] == 0 ||
                        SequenceHelpers.IsNewer(minMaxSnapshotTick[minMaxOffset], latestTick))
                    {
                        minMaxSnapshotTick[minMaxOffset] = latestTick;
                    }

                    if (minMaxSnapshotTick[minMaxOffset + 1] == 0 ||
                        SequenceHelpers.IsNewer(latestTick, minMaxSnapshotTick[minMaxOffset + 1]))
                    {
                        minMaxSnapshotTick[minMaxOffset + 1] = latestTick;
                    }
                }
#endif
                NetCodeTestGhostObjectSnapshotData snapshotData;
                snapshot.GetDataAtTick(targetTick, targetTickFraction, out snapshotData);

                Rotation ghostRotation = ghostRotationArray[entityIndex];
                Translation ghostTranslation = ghostTranslationArray[entityIndex];
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostRotationArray[entityIndex] = ghostRotation;
                ghostTranslationArray[entityIndex] = ghostTranslation;
            }
        }
    }

    [BurstCompile]
    private struct UpdatePredictedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<uint> minMaxSnapshotTick;
#endif
#pragma warning disable 649
        [NativeSetThreadIndex] public int ThreadIndex;
#pragma warning restore 649
        [NativeDisableParallelForRestriction] public NativeArray<uint> minPredictedTick;
        [ReadOnly] public ArchetypeChunkBufferType<NetCodeTestGhostObjectSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<PredictedGhostComponent> predictedGhostComponentType;
        public ArchetypeChunkComponentType<Rotation> ghostRotationType;
        public ArchetypeChunkComponentType<Translation> ghostTranslationType;
        public uint targetTick;
        public uint lastPredictedTick;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            GhostDeserializerState deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            NativeArray<Entity> ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            BufferAccessor<NetCodeTestGhostObjectSnapshotData> ghostSnapshotDataArray =
                chunk.GetBufferAccessor(ghostSnapshotDataType);
            NativeArray<PredictedGhostComponent> predictedGhostComponentArray =
                chunk.GetNativeArray(predictedGhostComponentType);
            NativeArray<Rotation> ghostRotationArray = chunk.GetNativeArray(ghostRotationType);
            NativeArray<Translation> ghostTranslationArray = chunk.GetNativeArray(ghostTranslationType);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            int minMaxOffset = ThreadIndex * (JobsUtility.CacheLineSize / 4);
#endif
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                DynamicBuffer<NetCodeTestGhostObjectSnapshotData> snapshot = ghostSnapshotDataArray[entityIndex];
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                uint latestTick = snapshot.GetLatestTick();
                if (latestTick != 0)
                {
                    if (minMaxSnapshotTick[minMaxOffset] == 0 ||
                        SequenceHelpers.IsNewer(minMaxSnapshotTick[minMaxOffset], latestTick))
                    {
                        minMaxSnapshotTick[minMaxOffset] = latestTick;
                    }

                    if (minMaxSnapshotTick[minMaxOffset + 1] == 0 ||
                        SequenceHelpers.IsNewer(latestTick, minMaxSnapshotTick[minMaxOffset + 1]))
                    {
                        minMaxSnapshotTick[minMaxOffset + 1] = latestTick;
                    }
                }
#endif
                NetCodeTestGhostObjectSnapshotData snapshotData;
                snapshot.GetDataAtTick(targetTick, out snapshotData);

                PredictedGhostComponent predictedData = predictedGhostComponentArray[entityIndex];
                uint lastPredictedTickInst = lastPredictedTick;
                if (lastPredictedTickInst == 0 || predictedData.AppliedTick != snapshotData.Tick)
                {
                    lastPredictedTickInst = snapshotData.Tick;
                }
                else if (!SequenceHelpers.IsNewer(lastPredictedTickInst, snapshotData.Tick))
                {
                    lastPredictedTickInst = snapshotData.Tick;
                }

                if (minPredictedTick[ThreadIndex] == 0 ||
                    SequenceHelpers.IsNewer(minPredictedTick[ThreadIndex], lastPredictedTickInst))
                {
                    minPredictedTick[ThreadIndex] = lastPredictedTickInst;
                }

                predictedGhostComponentArray[entityIndex] = new PredictedGhostComponent
                    { AppliedTick = snapshotData.Tick, PredictionStartTick = lastPredictedTickInst };
                if (lastPredictedTickInst != snapshotData.Tick)
                {
                    continue;
                }

                Rotation ghostRotation = ghostRotationArray[entityIndex];
                Translation ghostTranslation = ghostTranslationArray[entityIndex];
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostRotationArray[entityIndex] = ghostRotation;
                ghostTranslationArray[entityIndex] = ghostTranslation;
            }
        }
    }
}

public class NetCodeTestGhostObjectGhostSpawnSystem : DefaultGhostSpawnSystem<NetCodeTestGhostObjectSnapshotData>
{
}