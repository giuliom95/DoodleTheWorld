using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteHandler : MonoBehaviour
{
    public GameObject palette;
    public GameObject colorIndicator;
    public MainLogic mainLogic;

    public void TogglePalette()
    {
        palette.SetActive(!palette.activeSelf);
    }

    public void ChangeColor(GameObject btn)
    {
        Color c = btn.GetComponent<Image>().color;
        colorIndicator.GetComponent<Image>().color = c;
        int paletteMaterialId = int.Parse(btn.name.Substring(5));
        mainLogic.changePaletteMaterial(paletteMaterialId);
        TogglePalette();
    }
}
