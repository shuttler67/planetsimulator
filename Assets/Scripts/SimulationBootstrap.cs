using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using VerletEngine;
using Random = UnityEngine.Random;

public class SimulationBootstrap
{
    public static string SettingsGameObjectName = "Settings";

    public static SimulationSettings SimulationSettings { get; private set; }
    public static SpaceDistortSettings SpaceDistortSettings { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeSimulation()
    {
        var settingsGo = GameObject.Find(SettingsGameObjectName);
        if (settingsGo == null)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        StartSimulation();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartSimulation();
    }

    private static void StartSimulation()
    {
        var settingsGO = GameObject.Find(SettingsGameObjectName);
        SimulationSettings = settingsGO.GetComponent<SimulationSettings>();
        SpaceDistortSettings = settingsGO.GetComponent<SpaceDistortSettings>();

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var sun = entityManager.CreateEntity();
        entityManager.AddComponentData(sun, new Position());

        using (var instances = new NativeArray<Entity>(SimulationSettings.SimSize, Allocator.Temp))
        {
            entityManager.Instantiate(SimulationSettings.planetoidPrefab, instances);
            var asteroidTransform = SimulationSettings.planetoidPrefab.transform;
            float3 position = asteroidTransform.position;

            for (var i = 0; i < instances.Length; ++i)
            {
                position += math.cross(Random.insideUnitSphere, SimulationSettings.Spread);

                entityManager.SetComponentData(instances[i], new Position2D { Value = position.xy });
                float2 velocity = ((float3)(asteroidTransform.forward) * (SimulationSettings.AsteroidSpeed + Random.Range(-SimulationSettings.StartSpeedRandomness, SimulationSettings.StartSpeedRandomness))).xz;

                
                if (entityManager.HasComponent<OldPosition2D>(instances[i]))
                {
                    entityManager.SetComponentData(instances[i], new OldPosition2D { Value = position.xy -velocity*Time.fixedDeltaTime });
                }
                entityManager.SetComponentData(instances[i], new Mass { Value = Random.Range(-SimulationSettings.MassRandomness, SimulationSettings.MassRandomness) + (asteroidTransform.localScale.x * asteroidTransform.localScale.x) });

                entityManager.AddComponentData(instances[i], new Parent() { Value = sun });
            }
        }
    }
}
