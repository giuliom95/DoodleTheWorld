using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class cameraHandling : MonoBehaviour
{
    public string apiURL = "https://giuliom95.pythonanywhere.com";
    public Text coordText;
    public GameObject markerPrefab;
    public GameObject pointPrefab;
    public GameObject edgePrefab;

    private List<Stroke> strokes;
    private Stroke currentStroke;

    private Camera cam;

    private GameObject markerInstance;
    private string markerId;
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
                    markerId = img.Name;
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

                    StartCoroutine(LoadArea(markerId));
                }
                else
                {
                    Touch touch = Input.GetTouch(0);
                    Matrix4x4 m = cam.projectionMatrix.inverse;
                    Vector2 screenPos = touch.position;

                    Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .3f));

                    if (touch.phase == TouchPhase.Began)
                    {
                        currentStroke = new Stroke(p, pointPrefab, edgePrefab, markerInstance);
                        strokes.Add(currentStroke);
                    }
                    else
                        currentStroke.AddSegment(p);
                }
            }
        }
    }

    IEnumerator LoadArea(string areaId)
    {
        string url = apiURL + "/" + areaId;
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.isNetworkError)
        {
            coordText.text = "Net: " + req.error + "\nCode: " + req.responseCode;
        }
        else if (req.isHttpError)
        {
            coordText.text = "HTTP: " + req.error;
        }
        else
        {
            var areaData = JsonUtility.FromJson<SerializableArea>(req.downloadHandler.text);
            foreach (SerializableDrawing d in areaData.drawings)
            {
                foreach (SerializableStroke s in d.strokes)
                {
                    new Stroke(s, pointPrefab, edgePrefab, markerInstance);
                }
            }
        }
    }


}