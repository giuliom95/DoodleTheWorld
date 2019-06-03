using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeTest : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject edgePrefab;
    public GameObject worldOrigin;

    public TextAsset inputJSON;

    SerializableDrawing drawing;

    void Start()
    {
        drawing = JsonUtility.FromJson<SerializableDrawing>(inputJSON.text);
        foreach(var s in drawing.strokes)
        {
            new Stroke(s, pointPrefab, edgePrefab, worldOrigin);
            StrokeAround();
        }
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
        Vector3 pnt = Random.insideUnitSphere;
        Vector3 dir = Random.onUnitSphere;
        Stroke stroke = new Stroke(pnt, pointPrefab, edgePrefab, worldOrigin);
        for (int i = 0; i < 10; ++i)
        {
            dir += 0.7f * Random.insideUnitSphere;
            dir.Normalize();
            pnt += 0.1f * dir;
            stroke.AddSegment(pnt);
        }
        //drawing.Add(stroke);
    }
}
