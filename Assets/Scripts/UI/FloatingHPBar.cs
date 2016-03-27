using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingHPBar : MonoBehaviour
{
    public Image image;
    public float fadeTime = 1;

    internal void setRatio(float ratio)
    {
        image.fillAmount = ratio;
        if (ratio == 0)
            gameObject.SetActive(false);
    }
}
