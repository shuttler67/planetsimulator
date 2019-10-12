using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformUpdateSystem))]
public class SpaceDistortSystem : JobComponentSystem
{
    //struct SpaceDistortData
    //{
    //    public readonly int Length;
    //    [ReadOnly] public ComponentDataArray<SpaceDistort> labels;
    //    public ComponentDataArray<float4x4> transforms;
    //}

    //[Inject] SpaceDistortData data;

    //[BurstCompile]
    //struct DistortSpaceJob : IJobParallelFor
    //{
    //    public SpaceDistortData data;

    //    public float spaceRadius;
    //    public float spaceXfactor;
    //    public float spaceYfactor;

    //    public void Execute(int i)
    //    {
    //        var transform = data.transforms[i].Value;
    //        var position = transform.c3.xy;

    //        var lengthSqr = math.lengthSquared(position);
    //        //position = math.select(position, position * (spaceRadius/ math.sqrt(lengthSqr)), lengthSqr > spaceRadius * spaceRadius);

    //        position *= new float2(spaceXfactor, spaceYfactor);

    //        transform.c3.xy = position;

    //        data.transforms[i] = new TransformMatrix() { Value = transform };
    //    }
    //}

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    var settings = SimulationBootstrap.SpaceDistortSettings;

    //    var jobHandle = new DistortSpaceJob()
    //    {
    //        data = data,
    //        spaceRadius  = settings.SpaceRadius.Value,
    //        spaceXfactor = settings.SpaceXfactor.Value,
    //        spaceYfactor = settings.SpaceYfactor.Value

    //    }.Schedule(data.Length, 64, inputDeps);

    //    return jobHandle;
    //}
}
