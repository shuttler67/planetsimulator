using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VerletEngine
{
    [UpdateAfter(typeof(PositionResolutionIterationSystem))]
    [UpdateInGroup(typeof(VerletEngineGroup))]
    public class VerletIntegrationSystem : JobComponentSystem
    {
        struct PointData
        {
            public readonly int Length;
            public ComponentDataArray<Position2D> positions;
            public ComponentDataArray<OldPosition2D> oldPositions;
            public ComponentDataArray<Acceleration2D> accelerations;
            [ReadOnly]
            public ComponentDataArray<VerletPoint> verletPoints;
        }
        [BurstCompile]
        struct IntegrationJob : IJobParallelFor
        {
            public PointData data;
            public float dt;
            public float oldDt;

            public void Execute(int i)
            {
                float2 position = data.positions[i].Value;

                data.positions[i] = new Position2D() { Value = position + (position - data.oldPositions[i].Value)*(dt/oldDt) + data.accelerations[i].Value * dt * dt };
                data.oldPositions[i] = new OldPosition2D() { Value = position };
                data.accelerations[i] = new Acceleration2D() { Value = 0 };
            }
        }

        [Inject] PointData data;

        float oldDt = Time.fixedDeltaTime;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //inputDeps = JobHandle.CombineDependencies(inputDeps, iterationSystem.inputDeps);

            var jobHandle = new IntegrationJob()
            {
                data = data,
                dt = Time.deltaTime, //* SimulationBootstrap.SimulationSettings.SimulationSpeed
                oldDt = oldDt 
            }.Schedule(data.Length, 16, inputDeps);

            oldDt = Time.deltaTime;
            return jobHandle;
        }
    }
}
