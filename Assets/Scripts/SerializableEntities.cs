using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableStroke
{
    public Vector3 origin;
    public List<Vector3> points;
}

[System.Serializable]
public class SerializableDrawing
{
    public string id;
    public List<SerializableStroke> strokes;
    
    public SerializableDrawing(string id)
    {
        this.id = id;
        strokes = new List<SerializableStroke>();
    }

    public void Add(Stroke s) {
        strokes.Add(s.BuildSerializable());
    }
}
