using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class MainLogic : MonoBehaviour
{
    public string apiURL = "https://giuliom95.pythonanywhere.com";
    public Text coordText;
    public GameObject buttons;
    public GameObject middleDot;
    public GameObject markerPrefab;
    public GameObject pointPrefab;
    public GameObject edgePrefab;
    public GameObject palette;

    public Material[] paletteMaterials;

    private SerializableDrawing drawing;
    private Stroke currentStroke;
    private Stack<Stroke> undoStack;

    private Camera cam;

    private GameObject markerPlaceholder;
    private GameObject marker;
    private string markerId;
    private bool markerIsDefinitive;

    private List<AugmentedImage> augmentedImages = new List<AugmentedImage>();

    private int currentPaletteMaterial = 0;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        drawing = new SerializableDrawing();
        undoStack = new Stack<Stroke>();
        cam = GetComponent<Camera>();
        markerPlaceholder = null;
        marker = null;
        markerIsDefinitive = false;
        coordText.text = "Point at marker";
        buttons.SetActive(false);
        middleDot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (!markerIsDefinitive)
        {
            Session.GetTrackables<AugmentedImage>(augmentedImages, TrackableQueryFilter.Updated);

            foreach (AugmentedImage img in augmentedImages)
            {
                if(markerPlaceholder == null)
                {
                    Anchor anchor = img.CreateAnchor(img.CenterPose);
                    markerPlaceholder = Instantiate(markerPrefab, anchor.transform);
                    markerId = img.Name;
                    coordText.text = "Tap when the red pyramid\nis aligned to the marker.";
                }
            }
        }
        
        if (Input.touchCount > 0)
        {
            // Waits for the user to close the palette before drawing
            if (palette.activeSelf)
                return;

            int id = Input.touches[0].fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                return;
            }

            if (markerPlaceholder != null)
            {
                if (!markerIsDefinitive)
                {
                    markerIsDefinitive = true;

                    Vector3 p = markerPlaceholder.transform.position;
                    Quaternion r = markerPlaceholder.transform.rotation;
                    //Destroy(markerPlaceholder);
                    markerPlaceholder.SetActive(false);
                    marker = new GameObject();
                    marker.transform.position = p;
                    marker.transform.rotation = r;

                    StartCoroutine(LoadArea());
                }
                else
                {
                    Touch touch = Input.GetTouch(0);
                    Matrix4x4 m = cam.projectionMatrix.inverse;

                    Vector3 p = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-0.001f, 0.002f, .3f));

                    if (touch.phase == TouchPhase.Began)
                        currentStroke = new Stroke(p, pointPrefab, edgePrefab, marker, paletteMaterials[currentPaletteMaterial]);
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        drawing.Add(currentStroke);
                        undoStack.Push(currentStroke);
                    }
                    else
                        currentStroke.AddSegment(p);
                }
            }
            else
            {
                coordText.text = "Marker not there";
            }
        }
    }

    public void changePaletteMaterial(int matId)
    {
        currentPaletteMaterial = matId;
    }

    public void UndoButtonClicked()
    {
        if(undoStack.Count > 0)
        {
            undoStack.Pop().Destroy();
            drawing.strokes.Remove(drawing.strokes[drawing.strokes.Count - 1]);
        }
    }

    public void SaveButtonClicked()
    {
        StartCoroutine(SaveDrawing());
    }

    public void ReloadBtnClicked()
    {
        buttons.SetActive(false);
        middleDot.SetActive(false);

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
                    var stroke = new Stroke(s, pointPrefab, edgePrefab, marker, paletteMaterials[s.color]);  
                }
            }
            buttons.SetActive(true);
            middleDot.SetActive(true);
            coordText.text = "";
        }
    }


}