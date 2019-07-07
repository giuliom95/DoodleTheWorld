using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashHandling : MonoBehaviour
{
    public Camera cam;
    public Animator uiAnimator;
    private Material videoMaterial;

    private float fadeStartTime;
    private bool fadeDone;
    private float fadeRatio;

    void Start()
    {
        videoMaterial = gameObject.GetComponent<Renderer>().material;
        videoMaterial.color = new Color(1, 1, 1, 1);

        fadeStartTime = 4.0f;
        fadeRatio = 0.015f;
        fadeDone = false;
    }

    void Update()
    {

        if (!fadeDone)
        {
            float curTime = Time.time;
            if(curTime > fadeStartTime)
            {
                Color c = videoMaterial.color;
                c.a -= fadeRatio;
                videoMaterial.color = c;

                if (c.a <= 0)
                {
                    fadeDone = true;
                    gameObject.SetActive(false);

                    uiAnimator.SetTrigger("EnterTrigger");
                }
            }
        }
    }
}
