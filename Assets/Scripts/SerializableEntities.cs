using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableStroke
{
    public Vector3          origin;
    public List<Vector3>    points;
    public int              color;
}

[System.Serializable]
public class SerializableDrawing
{
    public List<SerializableStroke> strokes;
    
    public SerializableDrawing()
    {
        strokes = new List<SerializableStroke>();
    }

    public void Add(Stroke s)
    {
        strokes.Add(s.BuildSerializable());
    }
}

[System.Serializable]
public class SerializableArea
{
    public List<SerializableDrawing> drawings;
}