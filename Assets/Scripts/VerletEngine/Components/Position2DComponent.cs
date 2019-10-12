using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VerletEngine
{
    public class Position2DComponent : ComponentDataWrapper<Position2D>
    {
    }
    [Serializable]
    public struct Position2D : IComponentData
    {
        public float2 Value;
    }
}
