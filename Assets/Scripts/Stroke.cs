using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stroke
{
    GameObject parent;
    GameObject pointPrefab;
    GameObject edgePrefab;
    List<GameObject> vertices;
    List<GameObject> edges;

    public Stroke(SerializableStroke s, GameObject pPfab, GameObject ePfab, GameObject marker)
    {
        pointPrefab = pPfab;
        edgePrefab = ePfab;
        vertices = new List<GameObject>();
        edges = new List<GameObject>();

        parent = new GameObject("Stroke");
        parent.transform.SetParent(marker.transform);
        parent.transform.position = s.origin;

        AddVertex(s.points[0], false);
        for (int i = 1; i < s.points.Count; ++i)
            AddSegment(s.points[i], false);
    }

    public Stroke(Vector3 firstPoint, GameObject pPfab, GameObject ePfab, GameObject marker)
    {
        pointPrefab = pPfab;
        edgePrefab = ePfab;
        vertices = new List<GameObject>();
        edges = new List<GameObject>();

        parent = new GameObject("Stroke");
        parent.transform.SetParent(marker.transform);
        parent.transform.position = firstPoint;

        AddVertex(firstPoint);
    }

    private void AddVertex(Vector3 v, bool worldPositionStays=true)
    {
        GameObject vGO = GameObject.Instantiate(pointPrefab, v, Quaternion.identity);
        vGO.transform.SetParent(parent.transform, worldPositionStays);
        vertices.Add(vGO);
    }

    public void AddSegment(Vector3 p, bool worldPositionStays = true)
    {
        Vector3 v0 = vertices[vertices.Count - 1].transform.localPosition;
        if (worldPositionStays)
            v0 = parent.transform.TransformPoint(v0);
        Vector3 v1 = p;
        Vector3 d = v1 - v0;
        float l = d.magnitude;
        Quaternion r = Quaternion.LookRotation(d);
        GameObject eGO = GameObject.Instantiate(edgePrefab, v0, r);
        eGO.transform.SetParent(parent.transform, worldPositionStays);
        Vector3 scale = eGO.transform.localScale;
        scale.z = l;
        eGO.transform.localScale = scale;
        edges.Add(eGO);

        AddVertex(v1, worldPositionStays);
    }

    public int Count
    {
        get { return vertices.Count; }
    }

    public SerializableStroke BuildSerializable()
    {
        SerializableStroke s = new SerializableStroke();
        s.origin = parent.transform.localPosition;
        s.points = new List<Vector3>();
        foreach(GameObject v in vertices)
            s.points.Add(v.transform.localPosition);
        return s;
    }
}