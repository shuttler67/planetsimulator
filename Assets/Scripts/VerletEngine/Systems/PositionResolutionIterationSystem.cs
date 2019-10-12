using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace VerletEngine
{
    [UpdateInGroup(typeof(VerletEngineGroup))]
    public class PositionResolutionIterationSystem : JobComponentSystem2
    {
        public int IterationCount = 7;

        ComponentGroup componentGroup;
        List<PositionResolutionSystem> positionResolutionSystems = new List<PositionResolutionSystem>();
        NativeMultiHashMap<Entity, float2>[] hashMaps;

        protected override void OnCreateManager(int capacity)
        {
            componentGroup = GetComponentGroup(typeof(Position2D), typeof(VerletPoint));
            hashMaps = new NativeMultiHashMap<Entity, float2>[IterationCount];
        }

        public void AddPositionResolutionSystem(PositionResolutionSystem system)
        {
            positionResolutionSystems.Add(system);
        }

        protected override void OnStartRunning()
        {
            int length = componentGroup.CalculateLength();

            for (int i = 0; i < IterationCount; i++)
            {
                hashMaps[i] = NativeManager.AllocateAndAutoDisposeNativeMultiHashMap<Entity, float2>(length * 12, Allocator.Persistent);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int length = componentGroup.CalculateLength();

            for (int j = 0; j < positionResolutionSystems.Count; j++)
            {
                inputDeps = JobHandle.CombineDependencies(inputDeps, positionResolutionSystems[j].GetDependencies());
            }
            inputDeps = JobHandle.CombineDependencies(inputDeps, componentGroup.GetDependency());


            bool firstUpdate = true;
            for (int i = 0; i < IterationCount; i++)
            {
                var positionOutput = hashMaps[i];
                for (int j = 0; j < positionResolutionSystems.Count; j++)
                {
                    inputDeps = positionResolutionSystems[j].OnUpdate(inputDeps, firstUpdate, positionOutput);
                }

                inputDeps = new ReadPositionOutput()
                {
                    positionInput = positionOutput,
                    positions = componentGroup.GetComponentDataArray<Position2D>(),
                    entities = componentGroup.GetEntityArray()
                }.Schedule(length, 64, inputDeps);

                JobHandle.ScheduleBatchedJobs();
                firstUpdate = false;
            }

            var handles = NativeManager.AllocateAndAutoDisposeNativeArray<JobHandle>(IterationCount, Allocator.Temp);
            for (int i = 0; i < IterationCount; i++)
            {
                handles[i] = new HashJobs.ClearMultiHashMapJob<Entity, float2>() { hashMap = hashMaps[i] }.Schedule(inputDeps);
            }

            return JobHandle.CombineDependencies(handles);
        }
        [BurstCompile]
        struct ReadPositionOutput : IJobParallelFor
        {
            public ComponentDataArray<Position2D> positions;
            [ReadOnly]
            public EntityArray entities;
            [ReadOnly]
            public NativeMultiHashMap<Entity, float2> positionInput;

            public void Execute(int index)
            {
                var entity = entities[index];
                bool success = positionInput.TryGetFirstValue(entity, out float2 value, out NativeMultiHashMapIterator<Entity> it);
                var position = positions[index].Value;
                while (success)
                {
                    position += value;
                    success = positionInput.TryGetNextValue(out value, ref it);
                }
                positions[index] = new Position2D() { Value = position };
                //positionInput.Remove(entity);
            }
        }
    }
}

//[DeallocateOnDestroyManager]
//NativeMultiHashMap<Entity, float> multiHashMap;
//[DeallocateOnDestroyManager]
//NativeHashMap<Entity, float> hashMap;
//NativeArray<float> array;
////[DeallocateOnDestroyManager]
////public NativeMultiHashMap<Entity, float> emultiHashMap;
////[DeallocateOnDestroyManager]
////public NativeHashMap<Entity, float> ehashMap;

//[BurstCompile]
//struct multiAdd : IJobParallelFor
//{
//    [WriteOnly]
//    public NativeMultiHashMap<Entity, float>.Concurrent multiHashMap;

//    public void Execute(int index)
//    {
//        multiHashMap.Add(new Entity() { Index = index }, 0);
//    }
//}

//PrevJobHandle = new Add()
//{
//    hashMap = hashMap
//            }.Schedule(10000, 64, PrevJobHandle);

//PrevJobHandle = new multiAdd()
//{
//    multiHashMap = multiHashMap
//            }.Schedule(10000, 64, PrevJobHandle);
//PrevJobHandle = new Read()
//{
//    hashMap = hashMap
//            }.Schedule(10000, 64, PrevJobHandle);
//PrevJobHandle = new multiReadFirst()
//{
//    multiHashMap = multiHashMap
//            }.Schedule(10000, 64, PrevJobHandle);

//PrevJobHandle = new multiAdd()
//{
//    multiHashMap = multiHashMap
//            }.Schedule(10000, 64, PrevJobHandle);

//PrevJobHandle = new multiReadAll()
//{
//    multiHashMap = multiHashMap,
//                array = array
//            }.Schedule(10000, 64, PrevJobHandle);
//[BurstCompile]
//struct Add : IJobParallelFor
//{
//    [WriteOnly]
//    public NativeHashMap<Entity, float>.Concurrent hashMap;

//    public void Execute(int index)
//    {
//        hashMap.TryAdd(new Entity() { Index = index }, 0);
//    }
//}
//[BurstCompile]
//struct multiReadFirst : IJobParallelFor
//{
//    [ReadOnly]
//    public NativeMultiHashMap<Entity, float> multiHashMap;

//    public void Execute(int index)
//    {
//        bool success = multiHashMap.TryGetFirstValue(new Entity() { Index = index }, out float value, out NativeMultiHashMapIterator<Entity> it);
//    }
//}
//[BurstCompile]
//struct multiReadAll : IJobParallelFor
//{
//    [ReadOnly]
//    public NativeMultiHashMap<Entity, float> multiHashMap;

//    [WriteOnly]
//    public NativeArray<float> array;

//    public void Execute(int index)
//    {
//        bool success = multiHashMap.TryGetFirstValue(new Entity() { Index = index }, out float value, out NativeMultiHashMapIterator<Entity> it);
//        while (success)
//        {
//            value += 1;
//            array[index] = value;
//            success = multiHashMap.TryGetNextValue(out value, ref it);
//        }
//    }
//}
//[BurstCompile]
//struct Read : IJobParallelFor
//{
//    [ReadOnly]
//    public NativeHashMap<Entity, float> hashMap;

//    public void Execute(int index)
//    {
//        hashMap.TryGetValue(new Entity() { Index = index }, out float value);
//        value += 1;
//    }
//}