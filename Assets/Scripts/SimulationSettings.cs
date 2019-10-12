﻿using Unity.Mathematics;
using UnityEngine;

public class SimulationSettings : MonoBehaviour
{
    public float GravityConstant = 9;
    public int SimSize = 1000;
    public float3 Spread = new float3(35, 35, 35);

    public float MassRandomness = 0.1f;
    public float AsteroidSpeed = 3;
    public float StartSpeedRandomness = 0.5f;
    public float SimulationSpeed = 7;

    public GameObject planetoidPrefab;
}
