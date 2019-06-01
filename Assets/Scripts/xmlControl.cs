using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;

public class xmlControl : MonoBehaviour
{
    private string path;
    // Start is called before the first frame update

    void Start()
    {
         path = string.Concat(Application.dataPath, "/Scripts/test.xml");
    }


    public List<List<Vector3>> SelectStroke(int ID)
    {
        XDocument xdoc = XDocument.Load(path);
        IEnumerable<XElement> query = from Marker in xdoc.Element("Drawings").Elements("Marker")
                                      where Marker.Attribute("ID").Value == ID.ToString()
                                    select Marker;

        List<List<Vector3>> drawing = new List<List<Vector3>>();
        foreach (XElement Drawing in query.Elements("Drawing"))
        {
            foreach (XElement strokes in Drawing.Elements("stroke"))
            {
                List<Vector3> stroke = new List<Vector3>();
                foreach (XElement position in strokes.Elements("position"))
                {
                    stroke.Add(new Vector3(float.Parse(position.Attribute("X").Value), float.Parse(position.Attribute("Y").Value), float.Parse(position.Attribute("Z").Value)));
                }                
                drawing.Add(stroke);
            }

        }

        return drawing;
    }

    public void CreateStroke(List<Vector3> points,int ID)
    {
        XDocument xdoc = XDocument.Load(path);
        //     Needs ID
        
        XElement root = new XElement("Drawing");
        XElement stroke = new XElement("stroke");
        for (int i = 0; i < (points.Count - 1); i++)
        {

            stroke.Add(new XElement("position", new XAttribute("X", points[i].x), new XAttribute("Y", points[i].y), new XAttribute("Z", points[i].z)));
        }

        root.Add(new XAttribute("ID", 21),stroke);
        xdoc.Element("Drawings").Elements("Marker").Where(x => x.Attribute("ID").Value == ID.ToString()).FirstOrDefault().Add(root);
        xdoc.Save(path);
    }

}

