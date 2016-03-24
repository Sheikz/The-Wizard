using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingHPBar : MonoBehaviour
{
    public Image image;

    internal void setRatio(float ratio)
    {
        image.fillAmount = ratio;
    }
}
