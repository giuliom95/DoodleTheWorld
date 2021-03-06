﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StrokeTest : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject edgePrefab;
    public GameObject worldOrigin;
    private Stack<Stroke> undoStack;

    public Material[] colors;

    public TextAsset inputJSON;

    SerializableDrawing drawing;
    string apiURL = "http://giuliom95.pythonanywhere.com/";

    void Start()
    {
        undoStack = new Stack<Stroke>();

        drawing = new SerializableDrawing();
        for(int i = 0; i < 20; ++i)
            StrokeAround();

        //StartCoroutine(LoadArea("test"));
        /*for (int i = 0; i < 10; ++i)
            StrokeAround();
        StartCoroutine(SaveDrawing("test"));*/
        /*
        Vector3 pnt = new Vector3(1, 0, 1);
        Stroke stroke = new Stroke(pnt, pointPrefab, edgePrefab, worldOrigin);
        stroke.AddSegment(pnt + new Vector3(1, 0, 0));
        drawing.Add(stroke);

        stroke = new Stroke(pnt + new Vector3(1, 0, 0), pointPrefab, edgePrefab, worldOrigin);
        stroke.AddSegment(pnt + new Vector3(2, 0, 0));
        drawing.Add(stroke);
        StartCoroutine(SaveDrawing("test"));
        */
    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Instance"))
                Destroy(obj);
            StartCoroutine(LoadArea("test"));
        }
        */
    }

    public void Undo()
    {
        Debug.Log("Undo start " + undoStack.Count + " " + drawing.strokes.Count);
        if (undoStack.Count > 0)
        {
            undoStack.Pop().Destroy();
            drawing.strokes.Remove(drawing.strokes[drawing.strokes.Count - 1]);
        }
        Debug.Log("Undo done " + undoStack.Count + " " + drawing.strokes.Count);
    }

    void StrokeAround()
    {
        Vector3 pnt = Random.insideUnitSphere;
        Vector3 dir = Random.onUnitSphere;
        Stroke stroke = new Stroke(pnt, pointPrefab, edgePrefab, worldOrigin, colors[Random.Range(0, 2)]);
        for (int i = 0; i < 10; ++i)
        {
            dir += 0.7f * Random.insideUnitSphere;
            dir.Normalize();
            pnt += 0.1f * dir;
            stroke.AddSegment(pnt);
        }
        drawing.Add(stroke);
        undoStack.Push(stroke);
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
                    new Stroke(s, pointPrefab, edgePrefab, worldOrigin, colors[1]);
                }
            }
            Debug.Log("Loaded");
        }
    }

    IEnumerator SaveDrawing(string areaId)
    {
        string jsonData = JsonUtility.ToJson(drawing);   
        string url = apiURL + "/" + areaId;
        UnityWebRequest req = UnityWebRequest.Put(url, jsonData);
        yield return req.SendWebRequest();

        if (req.isNetworkError)
        {
            Debug.Log("Net: " + req.error + "\nCode: " + req.responseCode);
        }
        else if (req.isHttpError)
        {
            Debug.Log("HTTP: " + req.error);
        }
        else
        {
            Debug.Log("Saved!");
        }
    }
}
