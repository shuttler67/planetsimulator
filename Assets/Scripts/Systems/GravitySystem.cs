using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VerletEngine;

[UpdateInGroup(typeof(VerletEngineGroup))]
public class GravitySystem : JobComponentSystem
{
    [BurstCompile]
    struct VerletGravityJob : IJobParallelFor
    {
        public StarData stars;
        public VerletPointData points;
        public float G;

        public void Execute(int index)
        {
            var acceleration = points.accelerations[index].Value;
            var mass = points.masses[index].Value;

            for (int i = 0; i < stars.Length; i++)
            {
                var v = stars.positions[i].Value - points.positions[index].Value;
                var f = G * ((stars.masses[i].Value * mass) / math.lengthSquared(v));
                acceleration += (math.normalize(v) * f) / mass;
            }

            points.accelerations[index] = new Acceleration2D { Value = acceleration };
        }
    }

    struct StarData
    {
        public readonly int Length;
        public GameObjectArray starsGO;
        [ReadOnly] public ComponentDataArray<Star> stars;
        [ReadOnly] public ComponentDataArray<Position2D> positions;
        [ReadOnly] public ComponentDataArray<Mass> masses;
    }
    
    struct VerletPointData
    {
        public readonly int Length;
        [ReadOnly] public ComponentDataArray<VerletPoint> points;
        [ReadOnly] public ComponentDataArray<Position2D> positions;
        [ReadOnly] public ComponentDataArray<Mass> masses;
        public ComponentDataArray<Acceleration2D> accelerations;
    }

    [Inject] private StarData starsData;
    //[Inject] private PlanetoidData asteroidsData;
    [Inject] private VerletPointData pointData;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //var jobHandle = new GravityJob
        //{
        //    stars = starsData,
        //    planetoids = asteroidsData,
        //    dt = Time.deltaTime * SimulationBootstrap.SimulationSettings.SimulationSpeed,
        //    G = SimulationBootstrap.SimulationSettings.GravityConstant
        //}.Schedule(asteroidsData.Length, 64, inputDeps);

        inputDeps = new VerletGravityJob
        {
            stars = starsData,
            points = pointData,
            G = SimulationBootstrap.SimulationSettings.GravityConstant
        }.Schedule(pointData.Length, 64, inputDeps);
        
        return inputDeps;
    }
}