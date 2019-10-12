using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VerletEngine
{
    [Serializable]
    public struct Acceleration2D : IComponentData
    {
        public float2 Value;
    }

    public class Acceleration2DComponent : ComponentDataWrapper<Acceleration2D>
    {
    }
}
