using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public GameObject GridCirclePrefab;

    public int circleCount = 10;

    public FloatReference maxRadius;
    public float minRadius = 0;
    public float maxYOffset = 5;
    public FloatReference GridSpeed;
    public int smoothDegree = 4;
    public bool smoothStop = true;

    private DrawRadar[] circles;
    private float elapsedTime = 0;

	// Use this for initialization
	void Start () {
        circles = new DrawRadar[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            circles[i] = Instantiate(GridCirclePrefab).GetComponent<DrawRadar>();
        }
        Instantiate(GridCirclePrefab).GetComponent<DrawRadar>().Radius = maxRadius.Value;
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime*GridSpeed.Value;
        

        for (int i =0; i< circleCount; i++)
        {
            float offsettedTime = elapsedTime + i / (float)circleCount;

            circles[i].Radius = RadiusFunction(offsettedTime);
            circles[i].yOffset = -maxYOffset * SmoothStart(ClampTime(offsettedTime), smoothDegree);
        }
	}

    private float RadiusFunction(float offsettedTime)
    {
        float clampedTime = ClampTime(offsettedTime);

        float smoothedTime = smoothStop ? SmoothStop(clampedTime, smoothDegree) : SmoothStart(clampedTime, smoothDegree);


        return Mathf.Lerp(maxRadius.Value, minRadius, smoothedTime);
    }

    private float ClampTime(float time)
    {
        while (time > 1)
        {
            time--;
        }
        return time;
    }

    private float SmoothStop(float x, int degree)
    {       
        return 1 - SmoothStart(1-x, degree);
    }
    private float SmoothStart(float x, int degree)
    {
        for (int i = 1; i < degree; i++)
        {
            x *= x;
        }
        return x;
    }
}
