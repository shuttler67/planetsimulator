using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace VerletEngine
{

    public class DistanceConstraintSystem : PositionResolutionSystem
    {
        struct DistanceConstraints
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<DistanceConstraint> array;
        }
        
        [BurstCompile]
        struct ConstrainDistanceJob : IJobParallelFor
        {
            [ReadOnly]
            public DistanceConstraints constraints;
            [ReadOnly]
            public ComponentDataFromEntity<Position2D> positions;

            [WriteOnly]
            public NativeMultiHashMap<Entity, float2>.Concurrent positionOutput;
            

            public void Execute(int i)
            {
                var constraint = constraints.array[i];
                var entity1 = constraint.point1;
                var entity2 = constraint.point2;

                float2 pos1 = positions[entity1].Value;
                float2 pos2 = positions[entity2].Value;

                var diff = pos1 - pos2;
                var dist = math.length(diff);

                diff = math.select(diff / dist, 0, dist < 0.001);
                diff *= (constraint.distance - dist) * 0.5f;

                positionOutput.Add(entity1, diff);
                positionOutput.Add(entity2, -diff);
            }
        }
        [Inject] ComponentDataFromEntity<Position2D> positions;
        [Inject] DistanceConstraints distanceConstraints;
        

        protected override JobHandle OnResolutionUpdate(JobHandle inputDeps, NativeMultiHashMap<Entity, float2> positionOutput)
        {
            inputDeps = new ConstrainDistanceJob()
            {
                constraints = distanceConstraints,
                positionOutput = positionOutput,
                positions = positions,
            }.Schedule(distanceConstraints.Length, 64, inputDeps);
            return inputDeps;
        }
    }
}



//struct Slice
//{
//    public int Stride;
//    public int Length;

//    public override string ToString()
//    {
//        return String.Format("({0},{1})", Stride, Length);
//    }
//}
//struct Output
//{
//    public Entity first;
//    public float2 Value;

//    public override string ToString()
//    {
//        return String.Format("({0},{1})", first, Index);
//    }
//}

//[BurstCompile]
//struct ReadJob : IJobParallelFor
//{
//    public ComponentDataArray<Position2D> positions;

//    [ReadOnly]
//    public NativeArray<Slice> slicesOfIndices;

//    [ReadOnly]
//    public NativeArray<OutputIndex> constrainJobOutputIndices;

//    [ReadOnly]
//    public NativeArray<float2> constrainJobOutput;


//    public void Execute(int posIndex)
//    {
//        float2 position = positions[posIndex].Value;
//        Slice slice = slicesOfIndices[posIndex];

//        for (int i = slice.Stride; i < slice.Stride + slice.Length; i++)
//        {
//            var JobOutputIndex = constrainJobOutputIndices[i];
//            float2 diff = constrainJobOutput[JobOutputIndex.Index];
//            position += math.select(-diff, diff, JobOutputIndex.IsFirst);
//        }
//        positions[posIndex] = new Position2D() { Value = position };
//    }
//}
//[BurstCompile]
//struct ResetJob : IJob
//{
//    [ReadOnly]
//    public DistanceConstraints distanceConstraints;

//    public NativeArray<Slice> slicesOfIndices;
//    [WriteOnly]
//    public NativeArray<OutputIndex> constrainJobOutputIndices;
//    public NativeArray<int> indexCount;
//    [ReadOnly]
//    public NativeHashMap<Entity, int> hashMap;

//    public void Execute()
//    {
//        for (int i = 0; i < distanceConstraints.Length; i++)
//        {
//            var constraint = distanceConstraints.array[i];
//            hashMap.TryGetValue(constraint.point1, out int index1);
//            hashMap.TryGetValue(constraint.point2, out int index2);

//            var slice1 = slicesOfIndices[index1];
//            var slice2 = slicesOfIndices[index2];

//            constrainJobOutputIndices[slice1.Stride + indexCount[index1]] = new OutputIndex() { IsFirst = true, Index = i };
//            constrainJobOutputIndices[slice2.Stride + indexCount[index2]] = new OutputIndex() { IsFirst = false, Index = i };
//            indexCount[index1]++;
//            indexCount[index2]++;
//        }
//    }
//}
//[BurstCompile]
//struct HashJob : IJobParallelFor
//{
//    [ReadOnly]
//    public EntityArray entities;

//    public NativeHashMap<Entity, int>.Concurrent hashMap;

//    public void Execute(int i)
//    {
//        hashMap.TryAdd(entities[i], i);
//    }
//}


//[DeallocateOnDestroyManager]
//private NativeArray<Slice> slicesOfIndices;

//[DeallocateOnDestroyManager]
//private NativeArray<OutputIndex> constrainJobOutputIndices;

//[DeallocateOnDestroyManager]
//private NativeArray<float2> constrainJobOutput;

//[DeallocateOnDestroyManager]
//NativeArray<int> indexCount;

//[DeallocateOnDestroyManager]
//NativeHashMap<Entity, int> hashMap;