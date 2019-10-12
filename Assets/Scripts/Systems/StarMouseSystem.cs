using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using VerletEngine;
using UnityEngine;

namespace Assets.Scripts
{
    public class StarMouseDownSystem : ComponentSystem
    {
        struct StarMouseData
        {
            public readonly int Length;
            [ReadOnly]public ComponentDataArray<StarMouse> starMouse;
            [ReadOnly] public EntityArray entities;
            public SubtractiveComponent<Star> noStar;
        }
        [Inject] StarMouseData starMouse;
        [Inject] EntityManager entityManager;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                for(int i = 0; i < starMouse.Length; i++)
                {
                    entityManager.AddComponentData(starMouse.entities[i], new Star());
                }
            }
        }
    }
    public class StarMouseDragSystem : ComponentSystem
    {
        struct StarMouseData
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<StarMouse> starMouse;
            [ReadOnly] public ComponentDataArray<Star> star;
            [WriteOnly] public ComponentDataArray<Position2D> positions;

        }
        [Inject] StarMouseData starMouse;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePos = Input.mousePosition;

                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                for (int i = 0; i < starMouse.Length; i++)
                {
                    starMouse.positions[i] = new Position2D() { Value = new float3(mousePos).xy };
                }
            }
        }
    }
    public class StarMouseUpSystem : ComponentSystem
    {
        struct StarMouseData
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<StarMouse> starMouse;
            [ReadOnly] public ComponentDataArray<Star> star;
            [ReadOnly] public EntityArray entities;

        }
        [Inject] StarMouseData starMouse;
        [Inject] EntityManager entityManager;

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(0))
            {
                for (int i = 0; i < starMouse.Length; i++)
                {
                    entityManager.RemoveComponent<Star>(starMouse.entities[i]);
                }
            }
        }
    }
}
