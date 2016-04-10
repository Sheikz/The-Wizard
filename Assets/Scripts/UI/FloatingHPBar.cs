using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingHPBar : MonoBehaviour
{
    public Image image;
    public float fadeTime = 1;

    private Image background;

    void Awake()
    {
        background = GetComponent<Image>();
    }

    internal void setRatio(float ratio)
    {
        image.fillAmount = ratio;
        if (ratio == 0)
            gameObject.SetActive(false);
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
