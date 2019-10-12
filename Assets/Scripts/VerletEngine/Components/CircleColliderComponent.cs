using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Entities;

namespace VerletEngine
{
    public class CircleColliderComponent : ComponentDataWrapper<CircleCollider>
    {
    }

    [Serializable]
    public struct CircleCollider : IComponentData
    {
        public float Radius;
    }
}
