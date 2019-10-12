
using Unity.Entities;

namespace VerletEngine
{
    public class DistanceConstrainedPointComponent : ComponentDataWrapper<DistanceConstrainedPoint>
    {
    }
    public struct DistanceConstrainedPoint : IComponentData
    {
        public int NumberConstraints;
    }
}
