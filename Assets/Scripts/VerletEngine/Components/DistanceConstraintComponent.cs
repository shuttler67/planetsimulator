using System;
using Unity.Entities;

namespace VerletEngine
{
    public class DistanceConstraintComponent : ComponentDataWrapper<DistanceConstraint>
    {
    }
    [Serializable]
    public struct DistanceConstraint : IComponentData
    {
        public Entity point1;
        public Entity point2;
        public float distance;
    }
    
}
