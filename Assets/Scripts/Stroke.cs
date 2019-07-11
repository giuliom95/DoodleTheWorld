using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stroke
{
    GameObject parent;
    GameObject pointPrefab;
    GameObject edgePrefab;
    public List<GameObject> vertices;
    List<GameObject> edges;
    Material material;

    public Stroke(SerializableStroke s, GameObject pPfab, GameObject ePfab, GameObject marker, Material mat)
    {
        material = mat;
        pointPrefab = pPfab;
        edgePrefab = ePfab;
        vertices = new List<GameObject>();
        edges = new List<GameObject>();

        parent = new GameObject("Stroke");
        parent.transform.position = s.origin;

        AddVertex(s.points[0], false);
        for (int i = 1; i < s.points.Count; ++i)
            AddSegment(s.points[i], false);

        parent.transform.SetParent(marker.transform, false);
    }

    public Stroke(Vector3 firstPoint, GameObject pPfab, GameObject ePfab, GameObject marker, Material mat)
    {
        material = mat;

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
        vGO.GetComponent<Renderer>().material = material;
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
        eGO.GetComponent<Renderer>().material = material;
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
        s.color = int.Parse(material.name.Replace("_material", "").Replace("Color", ""));
        s.origin = parent.transform.localPosition;
        s.points = new List<Vector3>();
        var T = parent.transform.parent.transform;
        var p = T.InverseTransformPoint(vertices[0].transform.localPosition);
        foreach (GameObject v in vertices)
        {
            var Tv = T.InverseTransformPoint(v.transform.localPosition);
            s.points.Add(Tv - p);
        }
        return s;
    }

    public void Destroy()
    {
        foreach(GameObject v in vertices)
        {
            Object.Destroy(v);
        }

        foreach (GameObject e in edges)
        {
            Object.Destroy(e);
        }
    }
}