using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteHandler : MonoBehaviour
{
    public GameObject palette;

    public void OnClick()
    {
        Debug.Log("CLICK");
        palette.SetActive(!palette.activeSelf);
    }
}
