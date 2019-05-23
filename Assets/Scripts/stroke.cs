using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stroke : MonoBehaviour
{
    public GameObject sphere;
    public GameObject cylinder;
    // Start is called before the first frame update
    void Start()
    {
        StrokeAround();
    }

    // Update is called once per frame
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
        List<Vector3> points = new List<Vector3>();
        Vector3 pnt = Random.insideUnitSphere;
        Vector3 dir = Random.onUnitSphere;
        points.Add(pnt);
        for (int i = 0; i < 4000; ++i)
        {
            dir += 0.7f * Random.insideUnitSphere;
            dir.Normalize();
            pnt += 0.1f * dir;
            points.Add(pnt);
        }


        Instantiate(sphere, points[0], Quaternion.identity);
        for (int i = 0; i < (points.Count - 1); ++i)
        {
            Vector3 v0 = points[i];
            Vector3 v1 = points[i + 1];
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
