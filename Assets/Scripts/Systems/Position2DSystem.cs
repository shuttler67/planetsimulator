using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using VerletEngine;
using Unity.Transforms;

[UpdateAfter(typeof(VerletEngineGroup))]
public class TransformUpdateSystem : JobComponentSystem
{
    [Inject]
    PositionData data;

    struct PositionData
    {
        public readonly int Length;
        [ReadOnly] public ComponentDataArray<Position2D> positions2d;
        [WriteOnly] public ComponentDataArray<Position> positions;
    }
    [BurstCompile]
    struct CopyPosition2DDataToPosition : IJobParallelFor
    {
        public PositionData data;

        public void Execute(int i)
        {
            var position = data.positions2d[i].Value;
            position = math.select(position, new float2(), float.IsNaN(position.x) || float.IsNaN(position.y));
            

            data.positions[i] = new Position() { Value = new float3(position, 1) };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new CopyPosition2DDataToPosition()
        {
            data = data,
        }.Schedule(data.Length, 32, inputDeps);

        return jobHandle;
    }
}
