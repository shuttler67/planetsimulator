using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VelocityComponent : ComponentDataWrapper<Velocity2D>
{
}
[Serializable]
public struct Velocity2D : IComponentData
{
    public float2 Value;
}
