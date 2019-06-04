using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StrokeTest : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject edgePrefab;
    public GameObject worldOrigin;

    public TextAsset inputJSON;

    SerializableDrawing drawing;

    void Start()
    {
        Debug.Log("Hallo #1");
        StartCoroutine(LoadArea("test"));
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

    IEnumerator LoadArea(string areaId)
    {
        UnityWebRequest req = UnityWebRequest.Get("http://giuliom95.pythonanywhere.com/" + areaId);
        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else
        {
            var areaData = JsonUtility.FromJson<SerializableArea>(req.downloadHandler.text);
            foreach (SerializableDrawing d in areaData.drawings)
            {
                foreach (SerializableStroke s in d.strokes)
                {
                    new Stroke(s, pointPrefab, edgePrefab, worldOrigin);
                }
            }
        }
    }
}
