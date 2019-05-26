using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Stroke
{

    public GameObject parent;
    GameObject pointPrefab;
    GameObject edgePrefab;
    List<GameObject> points;
    List<GameObject> edges;

    public Stroke(Vector3 firstPoint, GameObject pPfab, GameObject ePfab)
    {
        pointPrefab = pPfab;
        edgePrefab = ePfab;
        points = new List<GameObject>();
        edges = new List<GameObject>();

        AddPoint(firstPoint);
        parent = new GameObject("Stroke");
        parent.transform.position = firstPoint;

    }

    private void AddPoint(Vector3 p)
    {
        GameObject go = GameObject.Instantiate(pointPrefab, p, Quaternion.identity);
        points.Add(go);
    }

    public void AddSegment(Vector3 p)
    {
        Vector3 v0 = this[points.Count - 1];
        Vector3 v1 = p;
        Vector3 d = v1 - v0;
        float l = d.magnitude;
        Quaternion r = Quaternion.LookRotation(d);
        GameObject c = GameObject.Instantiate(edgePrefab, v0, r);
        Vector3 scale = c.transform.localScale;
        scale.z = l;
        c.transform.localScale = scale;

        AddPoint(p);
    }

    public Vector3 this[int i]
    {
        get { return points[i].transform.position; }
    }

    public int Count
    {
        get { return points.Count; }
    }
}
