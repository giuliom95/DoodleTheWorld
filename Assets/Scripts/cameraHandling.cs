using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class cameraHandling : MonoBehaviour
{
    public Text coordText;
    public GameObject sphere;
    public GameObject cylinder;

    List<List<Vector3>> strokes;
    List<Vector3> currentStroke;

    Camera cam;

    Pose markerPose;
    string foundMarker;

    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();

    void Start()
    {
        strokes = new List<List<Vector3>>();
        cam = GetComponent<Camera>();
        foundMarker = null;
        coordText.text = "Looking for marker...";
    }

    // Update is called once per frame
    void Update()
    {
        Session.GetTrackables<AugmentedImage>(augmentedImages, TrackableQueryFilter.Updated);

        foreach(AugmentedImage img in augmentedImages)
        {
            markerPose = img.CenterPose;
            if (foundMarker == null)
                foundMarker = img.Name;
            coordText.text = "Marker " + foundMarker + "\n" + markerPose.position + "\n" + markerPose.rotation.eulerAngles;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Matrix4x4 m = cam.projectionMatrix.inverse;
            Vector2 screenPos = touch.position;
            coordText.text = m.MultiplyPoint(new Vector3(screenPos.x, screenPos.y, .3f)) + "\n" + screenPos;

            Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .3f));
            //Vector2 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
                BeginStroke(p);
            else
                ContinueStroke(p);

            if (touch.phase == TouchPhase.Ended)
                {}
        }
    }

    void ContinueStroke(Vector3 p)
    {
        Vector3 v0 = currentStroke[currentStroke.Count - 1];
        Vector3 v1 = p;
        Vector3 d = v1 - v0;
        float l = d.magnitude;
        Quaternion r = Quaternion.LookRotation(d);
        GameObject c = Instantiate(cylinder, v0, r);
        Vector3 scale = c.transform.localScale;
        scale.z = l;
        c.transform.localScale = scale;
        Instantiate(sphere, v1, Quaternion.identity);
        currentStroke.Add(p);
    }

    void BeginStroke(Vector3 p)
    {
        currentStroke = new List<Vector3>();
        strokes.Add(currentStroke);
        currentStroke.Add(p);
    }
}
