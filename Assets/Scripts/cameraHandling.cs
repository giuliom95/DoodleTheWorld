using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraHandling : MonoBehaviour
{
    public Text coordText;
    public GameObject sphere;
    public GameObject cylinder;

    List<Vector3> points = new List<Vector3>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coordText.text = transform.position + "\n" + transform.rotation.eulerAngles;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .1f));
            //Vector2 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
                BeginStroke(p);

            if (touch.phase == TouchPhase.Moved)
                ContinueStroke(p);

            if (touch.phase == TouchPhase.Ended)
                {}
        }
    }

    void ContinueStroke(Vector3 p)
    {
        Vector3 v0 = points[points.Count - 1];
        Vector3 v1 = p;
        Vector3 d = v1 - v0;
        float l = d.magnitude;
        Quaternion r = Quaternion.LookRotation(d);
        GameObject c = Instantiate(cylinder, v0, r);
        Vector3 scale = c.transform.localScale;
        scale.z = l;
        c.transform.localScale = scale;
        Instantiate(sphere, v1, Quaternion.identity);
        points.Add(p);
    }

    void BeginStroke(Vector3 p)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Instance"))
            Destroy(obj);
        points.Clear();
        points.Add(p);
    }
}
