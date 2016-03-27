using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CastingBar : MonoBehaviour
{
    public Image image;

    internal void setRatio(float ratio)
    {
        image.fillAmount = ratio;
    }
}
