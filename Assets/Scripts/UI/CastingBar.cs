using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CastingBar : MonoBehaviour
{
    public Image image;

    private Image background;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    internal void setRatio(float ratio)
    {
        image.fillAmount = ratio;
    }

    public void setVisible(bool value)
    {
        float alphaToSet = 0;
        if (value)
            alphaToSet = 1f;

        Color tmp = background.color;
        tmp.a = alphaToSet;
        background.color = tmp;

        tmp = image.color;
        tmp.a = alphaToSet;
        image.color = tmp;
    }
}
