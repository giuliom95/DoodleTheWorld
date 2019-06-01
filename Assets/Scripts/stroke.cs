using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Stroke
{

    public GameObject parent;
    GameObject pointPrefab;
    GameObject edgePrefab;
    List<GameObject> vertices;
    List<GameObject> edges;

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

    private void AddVertex(Vector3 v)
    {
        GameObject vGO = GameObject.Instantiate(pointPrefab, v, Quaternion.identity, parent.transform);
        vertices.Add(vGO);
    }

    public void AddSegment(Vector3 p)
    {
        Vector3 v0 = this[vertices.Count - 1];
        Vector3 v1 = p;
        Vector3 d = v1 - v0;
        float l = d.magnitude;
        Quaternion r = Quaternion.LookRotation(d);
        GameObject eGO = GameObject.Instantiate(edgePrefab, v0, r, parent.transform);
        Vector3 scale = eGO.transform.localScale;
        scale.z = l;
        eGO.transform.localScale = scale;
        edges.Add(eGO);

        AddVertex(v1);
    }

    public Vector3 this[int i]
    {
        get { return vertices[i].transform.position; }
    }

    public int Count
    {
        get { return vertices.Count; }
    }
    void LoadStroke()
    {
        List<List<Vector3>> drawing = this.GetComponent<xmlControl>().SelectStroke(10);

        foreach (List<Vector3> stroke in drawing)
        {
            Debug.Log(stroke);
            Instantiate(sphere, stroke[0], Quaternion.identity);
            for (int i = 0; i < (stroke.Count - 1); ++i)
            {
                Debug.Log(stroke[i].x);
                Vector3 v0 = stroke[i];
                Vector3 v1 = stroke[i + 1];
                Vector3 d = v1 - v0;
                float l = d.magnitude;
                Quaternion r = Quaternion.LookRotation(d);
                GameObject c = Instantiate(cylinder, v0, r);
                Vector3 scale = c.transform.localScale;
                scale.z = l;
                c.transform.localScale = scale;
                Instantiate(sphere, v1, Quaternion.identity);
            }
        }
        

    }

}
