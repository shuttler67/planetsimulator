using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VerletEngine
{
    public struct VerletPoint : IComponentData
    {
    }

    public class VerletPointComponent : ComponentDataWrapper<VerletPoint>
    {

    }
}