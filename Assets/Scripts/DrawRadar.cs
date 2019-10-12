using UnityEngine;
using System.Collections;



public class DrawRadar : MonoBehaviour
{
    public float ThetaScale = 0.01f;

    public float Radius = 3f;
    public FloatReference xMultiplier;
    public FloatReference yMultiplier;
    public float xOffset = 0;
    public float yOffset = 0;

    private int Size;
    private LineRenderer LineDrawer;

    void Start()
    {
        LineDrawer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        var Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.positionCount = (Size);

        float increment = (2.0f * Mathf.PI * ThetaScale);

        for (int i = 0; i < Size; i++)
        {
            Theta += increment;
            float x = xMultiplier.Value * Radius * Mathf.Cos(Theta) + xOffset;
            float y = yMultiplier.Value * Radius * Mathf.Sin(Theta) + yOffset;
            LineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}