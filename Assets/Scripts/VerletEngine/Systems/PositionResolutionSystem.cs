using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace VerletEngine
{
    [UpdateInGroup(typeof(VerletEngineGroup))]
    [UpdateAfter(typeof(PositionResolutionIterationSystem))]
    [UpdateBefore(typeof(VerletIntegrationSystem))]
    public abstract class PositionResolutionSystem : JobComponentSystem2
    {
        protected PositionResolutionIterationSystem iterationSystem;


        protected override void OnCreateManager(int cap)
        {
            iterationSystem = World.Active.GetOrCreateManager<PositionResolutionIterationSystem>();
            iterationSystem.AddPositionResolutionSystem(this);
        }

        protected override void OnDestroyManager()
        {
            FieldInfo[] fieldInfo = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public); ;
            
            for (int i=0; i < fieldInfo.Length; i++)
            {
                if (fieldInfo[i].IsDefined(typeof(DeallocateOnDestroyManagerAttribute)))
                {
                    var value = fieldInfo[i].GetValue(this);
                    if (value is IDisposable)
                    {
                        ((IDisposable)value).Dispose();
                    }
                }
            }
        }

        //protected sealed override JobHandle OnUpdate(JobHandle inputDeps)
        //{
        //    return OnUpdate(inputDeps, false);
        //}

        public JobHandle OnUpdate(JobHandle inputDeps, bool isFirstUpdate, NativeMultiHashMap<Entity, float2> positionOutput)
        {
            if (isFirstUpdate)
                inputDeps = OnFirstUpdate(inputDeps);

            var d1 =  OnResolutionUpdate(inputDeps, positionOutput);
            return  OnResolutionUpdate(d1);
        }

        protected virtual JobHandle OnFirstUpdate(JobHandle inputDeps) { return inputDeps; }
        protected virtual JobHandle OnResolutionUpdate(JobHandle inputDeps, NativeMultiHashMap<Entity, float2> positionOutput) { return inputDeps; }
        protected virtual JobHandle OnResolutionUpdate(JobHandle inputDeps) { return inputDeps; }


        public JobHandle GetDependencies()
        {
            UpdateInjectedComponentGroups();
            JobHandle inputDeps = new JobHandle();
            foreach (var group in ComponentGroups)
            {
                inputDeps = JobHandle.CombineDependencies(inputDeps, group.GetDependency());
            }
            return inputDeps;
        }
    }
    //[UpdateAfter(typeof(TransformInputBarrier))]
    public class VerletEngineGroup {}

    
    
}
