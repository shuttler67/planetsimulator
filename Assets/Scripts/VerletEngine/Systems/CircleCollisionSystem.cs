using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using System;

namespace VerletEngine
{
    public class CircleCollisionSystem : PositionResolutionSystem, IDisposable
    {
        struct CircleData
        {
            public readonly int Length;
            public ComponentDataArray<Position2D> positions;
            public ComponentDataArray<OldPosition2D> oldPositions;
            
            [ReadOnly]
            public ComponentDataArray<CircleCollider> radii;
            [ReadOnly]
            public ComponentDataArray<VerletPoint> verletPoints;
        }

        struct Circle
        {
            public int index;
            public float2 position;
            public float radius;
        }
        
        [BurstCompile]
        struct ResolveCircleCollisionJob : IJobParallelFor
        {
            public ComponentDataArray<Position2D> positions;
            public ComponentDataArray<OldPosition2D> oldPositions;
            
            [ReadOnly]
            public ComponentDataArray<CircleCollider> radii;
            [ReadOnly]
            public NativeMultiHashMap<int, Circle> circleHashMap;
            [ReadOnly]
            public NativeArray<int2> cell2DOffsets;
            public float cellSize;
            public float collisionDampFactor;

            public Circle circle;
            

            public float2 ResolveCollision(float2 pos,float radius, int index, Circle circle)
            {
                radius += circle.radius;
                float2 pos2 = circle.position;
                float2 diff = pos - pos2;
                float distSqr = math.lengthSquared(diff);

                bool collision = distSqr < radius * radius;
                if (collision)
                {
                    float dist = math.sqrt(distSqr);
                    pos += diff * (radius - dist) / 2;

                    var oldPos = oldPositions[index].Value;
                    oldPositions[index] = new OldPosition2D() { Value = oldPos + diff * math.dot((pos - oldPos), (diff)) * collisionDampFactor };
                }
                return pos;
            }


            public void Execute(int index)
            {

                float2 position = positions[index].Value;
                float radius = radii[index].Radius;

                for (int i = 0; i < cell2DOffsets.Length; i++)
                {
                    var hashIndex = GridHash.Hash(position + new float2(cell2DOffsets[i])*cellSize, cellSize);

                    //Circle circle;
                    //NativeMultiHashMapIterator<int> it;
                    //for (int j = 0; j < positions.Length; j++)
                    //{
                        var k = radii[math.select(hashIndex,index, hashIndex >= radii.Length || hashIndex <0)];
                        k.Radius = 1;
                    //}                    
                    //bool success = circleHashMap.TryGetFirstValue(hashIndex, out circle, out it);

                    //while (success && index != circle.index)
                    //{
                    //    //position = ResolveCollision(position, radius, index, circle);
                    //    //position = math.select(position, ResolveCollision(position, radius, index, circle), success);

                    //    success = circleHashMap.TryGetNextValue(out circle, ref it);
                    //};
                    //for (int j = 0; j < 20; j++)
                    //{
                    //    position = ResolveCollision(position, radius, index, circle);
                    //}
                }
                positions[index] = new Position2D() { Value = position };
            }
        }
        [BurstCompile] 
        struct PopulateHashMapJob : IJobParallelFor
        {
            [WriteOnly]
            public NativeMultiHashMap<int, int>.Concurrent circleHashMap;
            [ReadOnly]
            public ComponentDataArray<Position2D> positions;
            //[ReadOnly]
            //public ComponentDataArray<CircleCollider> radii;

            public float cellSize;

            public void Execute(int i)
            {
                var hash = GridHash.Hash(positions[i].Value, cellSize);
                circleHashMap.Add(hash, i);//new Circle() { index = i, position = positions[i].Value, radius = radii[i].Radius });
            }
        }
        [BurstCompile]
        struct MergeCellIndices : IJobNativeMultiHashMapMergedSharedKeyIndices
        {
            public NativeArray<int> cellIndices;
            public NativeArray<int> cellCount;
            
            public void ExecuteFirst(int index)
            {
                cellIndices[index] = index;
                cellCount[index] = cellCount[index] + 1;
            }

            public void ExecuteNext(int firstIndex, int index)
            {
                cellIndices[index] = firstIndex;
                cellCount[index] = cellCount[index] + 1;
            }
        }


        //[Inject] CircleData circles;
        
        //[DeallocateOnDestroyManager]
        //NativeMultiHashMap<int, int> circleHashMap;
        //[DeallocateOnDestroyManager]
        //NativeArray<int2> cell2DOffsets;
        //NativeArray<int> cellCount;
        //NativeArray<int> cellIndices;
        //float cellSize = 1;
        //float collisionDampFactor = 0.01f;

        JobHandle prevJobHandle;

        protected override void OnDestroyManager()
        {
            prevJobHandle.Complete();
            base.OnDestroyManager();
            Dispose();
        }

        public void Dispose()
        {
            //if (circleHashMap.IsCreated)
            //    circleHashMap.Dispose();

            //if (cell2DOffsets.IsCreated)
            //    cell2DOffsets.Dispose();
        }

        protected override void OnCreateManager(int cap)
        {
            //cell2DOffsets = new NativeArray<int2>(GridHash.cell2DOffsets, Allocator.Persistent);
            //circleHashMap = new NativeMultiHashMap<int, int>(circles.Length, Allocator.Persistent);
            base.OnCreateManager(cap);
        }
        protected override JobHandle OnFirstUpdate(JobHandle inputDeps)
        {
            prevJobHandle.Complete();
           // circleHashMap.Capacity = circles.Length*2; 
            return inputDeps;
        }
        protected override JobHandle OnResolutionUpdate(JobHandle inputDeps, NativeMultiHashMap<Entity, float2> positionOutput)
        {
            //circleHashMap.Capacity = circles.Length;


            //circleHashMap.Clear();
            //circleHashMap.Capacity = circles.Length;
            //cellHashMap = new NativeMultiHashMap<int, int>(circles.Length, Allocator.TempJob);
            //cellIndices = new NativeArray<int>(circles.Length, Allocator.TempJob);
            //cellCount = new NativeArray<int>(circles.Length, Allocator.TempJob);


            //inputDeps = JobHandle.CombineDependencies(inputDeps, prevJobHandle);

            //inputDeps = new PopulateHashMapJob()
            //{
            //    positions = circles.positions,
            //    //radii = circles.radii,
            //    circleHashMap = circleHashMap,
            //    cellSize = cellSize
            //}.Schedule(circles.Length, 32, inputDeps);


            //prevJobHandle = new HashJobs.ClearMultiHashMapJob<int, int>()
            //{
            //    hashMap = circleHashMap,
            //}.Schedule(inputDeps);

            //inputDeps = new ResolveCircleCollisionJob()
            //{
            //    positions = circles.positions,
            //    oldPositions = circles.oldPositions,
            //    radii = circles.radii,
            //    circleHashMap = circleHashMap,
            //    cellSize = cellSize,
            //    cell2DOffsets = cell2DOffsets,
            //    collisionDampFactor = collisionDampFactor,
            //    circle = new Circle() { radius = 5, position = 1, index = 0}
            //}.Schedule(circles.Length, 32, inputDeps);
            

            return inputDeps;
        }


        //[BurstCompile]
        //struct MergeCellsJob : IJobNativeMultiHashMapMergedSharedKeyIndices
        //{
        //    public NativeArray<int> cellIndices;
        //    public NativeArray<int> cellCount;

        //    public NativeMultiHashMap<int, Circle>.Concurrent circleHashMap;
        //    [ReadOnly]
        //    public ComponentDataArray<Position2D> positions;
        //    [ReadOnly]
        //    public ComponentDataArray<CircleCollider> radii;

        //    public void ExecuteFirst(int index)
        //    {
        //        cellIndices[index] = index;
        //        circleHashMap.Add(index, new Circle() { index = index, position = positions[index].Value, radius = radii[index].Radius });
        //    }

        //    public void ExecuteNext(int cellIndex, int index)
        //    {
        //        cellCount[cellIndex] += 1;
        //        cellIndices[index] = cellIndex;
        //        circleHashMap.Add(cellIndex, new Circle() { index = index, position = positions[index].Value, radius = radii[index].Radius });
        //    }
        //}

    }
}
