using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using VerletEngine;

public class SpaceRadiusSystem : PositionResolutionSystem
{
    [Inject]
    PositionData data;

    struct PositionData
    {
        public readonly int Length;
        public ComponentDataArray<Position2D> positions;
        [ReadOnly]public ComponentDataArray<VerletPoint> verletPoints;
    }
    [BurstCompile]
    struct EnforceSpaceRadius : IJobParallelFor
    {
        public PositionData data;
        public float spaceRadius;

        public void Execute(int i)
        {
            var position = data.positions[i].Value;

            var length = math.length(position);
            position = math.select(position, position * (spaceRadius / length), length - 0.1 > spaceRadius);
            data.positions[i] = new Position2D() { Value = position };
        }
    }

    protected override JobHandle OnResolutionUpdate(JobHandle inputDeps)
    {
        var jobHandle = new EnforceSpaceRadius()
        {
            data = data,
            spaceRadius = SimulationBootstrap.SpaceDistortSettings.SpaceRadius.Value
        }.Schedule(data.Length, 16, inputDeps);

        return jobHandle;
    }
}
