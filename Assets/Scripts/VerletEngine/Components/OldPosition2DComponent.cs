using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VerletEngine
{
    public class OldPosition2DComponent : ComponentDataWrapper<OldPosition2D>
    {
    }
    [Serializable]
    public struct OldPosition2D : IComponentData
    {
        public float2 Value;
    }
}