using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class cameraHandling : MonoBehaviour
{
    public Text coordText;

    List<Stroke> strokes;
    Stroke currentStroke;

    Camera cam;

    Pose markerPose;
    string foundMarker;

    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();
    public GameObject pointPrefab;
    public GameObject edgePrefab;

    void Start()
    {
        strokes = new List<Stroke>();
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
            coordText.text = "HALLO";
            Touch touch = Input.GetTouch(0);
            Matrix4x4 m = cam.projectionMatrix.inverse;
            Vector2 screenPos = touch.position;

            //coordText.text = m.MultiplyPoint(new Vector3(screenPos.x, screenPos.y, .3f)) + "\n" + screenPos;
            //Vector2 pos = touch.position;

            Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .3f));

            if (touch.phase == TouchPhase.Began)
                BeginStroke(p);
            else
                currentStroke.AddSegment(p);

            coordText.text = "Count: " + currentStroke.Count;

            if (touch.phase == TouchPhase.Ended)
                {}
        }
    }

    void BeginStroke(Vector3 p)
    {
        currentStroke = new Stroke(p, pointPrefab, edgePrefab);
        strokes.Add(currentStroke);
    }
}