using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace VerletEngine
{
    public class DistanceConstraintAddAndRemoveSystem : JobComponentSystem
    {
        bool done = false;
        struct VerletPoints
        {
            public readonly int Length;
            [ReadOnly] public EntityArray entities;
            [ReadOnly] public ComponentDataArray<VerletPoint> verlets;
        }

        struct DistanceConstraints
        {
            public readonly int Length;
            public EntityArray entities;
            public ComponentDataArray<DistanceConstraint> constraints;
        }

        [Inject] VerletPoints verletPoints;
        
        [Inject] ComponentDataFromEntity<DistanceConstrainedPoint> constrainedPoints;

        private void AddConstraintToPoint (Entity point)
        {
            if (EntityManager.HasComponent<DistanceConstrainedPoint>(point))
                constrainedPoints[point] = new DistanceConstrainedPoint() { NumberConstraints = constrainedPoints[point].NumberConstraints + 1 };
            else
                EntityManager.AddComponentData(point, new DistanceConstrainedPoint() { NumberConstraints = 1});
        }

        private void RemoveConstraintFromPoint (Entity point)
        {
            var value = constrainedPoints[point].NumberConstraints - 1;

            if (value == 0)
                EntityManager.RemoveComponent<DistanceConstrainedPoint>(point);
            else
                constrainedPoints[point] = new DistanceConstrainedPoint() { NumberConstraints = value };
        }

        public void AddConstraint(DistanceConstraint distanceConstraint)
        {
            AddConstraintToPoint(distanceConstraint.point1);
            AddConstraintToPoint(distanceConstraint.point2);
            var e = EntityManager.CreateEntity();
            EntityManager.AddComponentData(e, distanceConstraint);
        }

        public void AddConstraints(NativeArray<DistanceConstraint> distanceConstraints)
        {
            var entities = new NativeArray<Entity>(distanceConstraints.Length, Allocator.Temp);
            EntityArchetype a = EntityManager.CreateArchetype(typeof(DistanceConstraint));
            EntityManager.CreateEntity(a, entities);

            constrainedPoints = EntityManager.GetComponentDataFromEntity<DistanceConstrainedPoint>();
            
            for (int i = 0; i < distanceConstraints.Length; i++)
            {
                var distanceConstraint = distanceConstraints[i];
                AddConstraintToPoint(distanceConstraint.point1);
                AddConstraintToPoint(distanceConstraint.point2);
                EntityManager.SetComponentData(entities[i], distanceConstraint);
            }
            entities.Dispose();
        }

        public void RemoveConstraint(Entity constraintEntity, DistanceConstraint constraint)
        {
            RemoveConstraintFromPoint(constraint.point1);
            RemoveConstraintFromPoint(constraint.point2);
            EntityManager.DestroyEntity(constraintEntity);
        }

        public void RemoveConstraints(NativeArray<Entity> constraintEntities, NativeArray<DistanceConstraint> constraints)
        {
            for (int i = 0; i < constraints.Length; i++)
            {
                RemoveConstraintFromPoint(constraints[i].point1);
                RemoveConstraintFromPoint(constraints[i].point2);
            }

            EntityManager.DestroyEntity(constraintEntities);
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!done)
            {
                var constraints = new NativeArray<DistanceConstraint>(verletPoints.entities.Length, Allocator.Temp);

                //for (int i = 0; i < verletPoints.entities.Length; i++)
                //{
                //    for (int j = i + 1; j < verletPoints.entities.Length; j++)
                //    {
                //        constraints[i] = (new DistanceConstraint() { point1 = verletPoints.entities[i], point2 = verletPoints.entities[j], distance = 5 });
                //    }
                //}
                for (int i = 0; i < verletPoints.entities.Length; i++)
                {
                    int j = math.select(i + 1, 0, i + 1 == verletPoints.entities.Length);
                    constraints[i] = (new DistanceConstraint() { point1 = verletPoints.entities[i], point2 = verletPoints.entities[j], distance = 5 });
                }
                done = true;

                AddConstraints(constraints);

                constraints.Dispose();
            }
            return JobHandle.CombineDependencies(inputDeps, ComponentGroups[0].GetDependency());
        }
    }
}
