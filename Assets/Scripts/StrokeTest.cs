using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeTest : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject edgePrefab;
    public GameObject worldOrigin;

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
            StrokeAround();
            StrokeAround();
            StrokeAround();
        }
    }

    void StrokeAround()
    {
        Vector3 pnt = Random.insideUnitSphere;
        Vector3 dir = Random.onUnitSphere;
        Stroke stroke = new Stroke(pnt, pointPrefab, edgePrefab, worldOrigin);
        for (int i = 0; i < 400; ++i)
        {
            dir += 0.7f * Random.insideUnitSphere;
            dir.Normalize();
            pnt += 0.1f * dir;
            stroke.AddSegment(pnt);
        }
        Debug.Log(JsonUtility.ToJson(stroke));
    }
}
