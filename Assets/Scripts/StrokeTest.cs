using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeTest : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject edgePrefab;

    void Start()
    {
        StrokeAround();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Instance"))
                Destroy(obj);
            StrokeAround();
        }
    }

    void StrokeAround()
    {
        // Vector3 pnt = Random.insideUnitSphere;
        // Vector3 dir = Random.onUnitSphere;
        // Stroke stroke = new Stroke(pnt, pointPrefab, edgePrefab);
        // for (int i = 0; i < 4000; ++i)
        // {
        //     dir += 0.7f * Random.insideUnitSphere;
        //     dir.Normalize();
        //     pnt += 0.1f * dir;
        //     stroke.AddSegment(pnt);
        // }
    }
}
