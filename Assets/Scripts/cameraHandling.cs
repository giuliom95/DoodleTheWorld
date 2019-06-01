using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

public class cameraHandling : MonoBehaviour
{
    public Text coordText;
    public GameObject markerPrefab;
    public GameObject pointPrefab;
    public GameObject edgePrefab;

    private List<Stroke> strokes;
    private Stroke currentStroke;

    private Camera cam;

    private GameObject markerInstance;
    private bool markerIsDefinitive;

    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();

    void Start()
    {
        strokes = new List<Stroke>();
        cam = GetComponent<Camera>();
        markerInstance = null;
        markerIsDefinitive = false;
        coordText.text = "Point the camera at the marker";
    }

    // Update is called once per frame
    void Update()
    {
        if(!markerIsDefinitive)
        {
            Session.GetTrackables<AugmentedImage>(augmentedImages, TrackableQueryFilter.Updated);

            foreach (AugmentedImage img in augmentedImages)
            {
                if(markerInstance == null)
                {
                    Anchor anchor = img.CreateAnchor(img.CenterPose);
                    markerInstance = Instantiate(markerPrefab, anchor.transform);
                    coordText.text = "Tap when the red pyramid\nis aligned to the marker.";
                }
            }
        }

        if (Input.touchCount > 0)
        {
            if (markerInstance != null)
            {
                if (!markerIsDefinitive)
                {
                    coordText.text = "";
                    markerIsDefinitive = true;
                    

                    Vector3 p = markerInstance.transform.position;
                    Quaternion r = markerInstance.transform.rotation;
                    Destroy(markerInstance);
                    markerInstance = Instantiate(markerPrefab);
                    markerInstance.transform.position = p;
                    markerInstance.transform.rotation = r;
                }
                else
                {
                    Touch touch = Input.GetTouch(0);
                    Matrix4x4 m = cam.projectionMatrix.inverse;
                    Vector2 screenPos = touch.position;

                    Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .3f));

                    if (touch.phase == TouchPhase.Began)
                        BeginStroke(p);
                    else
                        currentStroke.AddSegment(p);

                    //coordText.text = "Count: " + currentStroke.Count;
                }
            }
        }
    }

    void BeginStroke(Vector3 p)
    {
        currentStroke = new Stroke(p, pointPrefab, edgePrefab, markerInstance);
        strokes.Add(currentStroke);
    }
}