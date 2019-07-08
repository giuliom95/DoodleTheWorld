using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainLogic : MonoBehaviour
{
    public string apiURL = "https://giuliom95.pythonanywhere.com";
    public Text coordText;
    public GameObject buttons;
    public GameObject markerPrefab;
    public GameObject pointPrefab;
    public GameObject edgePrefab;

    public Material[] paletteMaterials;

    private SerializableDrawing drawing;
    private Stroke currentStroke;

    private Camera cam;

    private GameObject markerInstance;
    private string markerId;
    private bool markerIsDefinitive;

    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();

    private int currentPaletteMaterial = 0;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        drawing = new SerializableDrawing();
        cam = GetComponent<Camera>();
        markerInstance = null;
        markerIsDefinitive = false;
        coordText.text = "Point the camera at the marker";
        buttons.SetActive(false);
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
                    markerIsDefinitive = true;

                    Vector3 p = markerInstance.transform.position;
                    Quaternion r = markerInstance.transform.rotation;
                    Destroy(markerInstance);
                    markerInstance = Instantiate(markerPrefab);
                    markerInstance.transform.position = p;
                    markerInstance.transform.rotation = r;

                    StartCoroutine(LoadArea());
                }
                else
                {
                    Touch touch = Input.GetTouch(0);
                    Matrix4x4 m = cam.projectionMatrix.inverse;
                    Vector2 screenPos = touch.position;

                    Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, 0, .3f));

                    if (touch.phase == TouchPhase.Began)
                        currentStroke = new Stroke(p, pointPrefab, edgePrefab, markerInstance, paletteMaterials[currentPaletteMaterial]);
                    else if (touch.phase == TouchPhase.Ended)
                        drawing.Add(currentStroke);
                    else
                        currentStroke.AddSegment(p);
                }
            }
        }
    }

    public void changePaletteMaterial(int matId)
    {
        currentPaletteMaterial = matId;
    }

    public void SaveButtonClicked()
    {
        StartCoroutine(SaveDrawing());
    }

    public void ReloadBtnClicked()
    {
        buttons.SetActive(false);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Instance"))
            Destroy(obj);

        drawing = new SerializableDrawing();

        StartCoroutine(LoadArea());
    }

    IEnumerator SaveDrawing()
    {
        string jsonData = JsonUtility.ToJson(drawing);
        string url = apiURL + "/" + markerId;
        UnityWebRequest req = UnityWebRequest.Put(url, jsonData);
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
            coordText.text = "Saved!";
        }
    }

    IEnumerator LoadArea()
    {
        coordText.text = "Loading...";

        string url = apiURL + "/" + markerId;
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
                    var stroke = new Stroke(s, pointPrefab, edgePrefab, markerInstance, paletteMaterials[0]);  
                }
            }
            buttons.SetActive(true);
            coordText.text = "";
        }
    }


}