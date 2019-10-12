using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MassComponent : ComponentDataWrapper<Mass>
{
}
[Serializable]
public struct Mass : IComponentData
{
    public float Value;
}
