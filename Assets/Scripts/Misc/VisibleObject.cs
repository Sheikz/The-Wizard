using UnityEngine;
using System.Collections;

public abstract class VisibleObject : MonoBehaviour
{
    public float progressiveApparitionTime = 0.5f;
    private SpriteRenderer[] spriteRenderers;
    private Light[] lights;

    abstract public void setVisible();

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        lights = GetComponentsInChildren<Light>();

        foreach (SpriteRenderer spr in spriteRenderers)
        {
            spr.enabled = false;
        }
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }

    protected void setEnabled(bool value)
    {
        if (spriteRenderers.Length == 0)
            return;

        if (value)
            startProggressiveApparition();
        else
            startProggressiveDisparition();

        foreach (Light light in lights)
        {
            light.enabled = value;
        }
    }

    protected void startProggressiveApparition()
    {
        foreach (SpriteRenderer spr in spriteRenderers)
        {
            spr.enabled = true;
            if (progressiveApparitionTime <= 0)
                continue;
            StartCoroutine(appearAfterSeconds(progressiveApparitionTime, spr));
        }
    }

    private IEnumerator appearAfterSeconds(float duration, SpriteRenderer rdr)
    {
        float startingTime = Time.time;
        while (Time.time - startingTime < duration)
        {
            Color newColor = rdr.color;
            newColor.a = Mathf.Lerp(0, 1, (Time.time - startingTime) / duration);
            rdr.color = newColor;
            yield return null;
        }
        rdr.enabled = true;
    }

    protected void startProggressiveDisparition()
    {
        foreach (SpriteRenderer spr in spriteRenderers)
        {
            if (progressiveApparitionTime <= 0)
            {
                spr.enabled = false;
                continue;
            }
            StartCoroutine(disappearAfterSeconds(progressiveApparitionTime, spr));
        }
    }

    private IEnumerator disappearAfterSeconds(float duration, SpriteRenderer rdr)
    {
        float startingTime = Time.time;
        while (Time.time - startingTime < duration)
        {
            Color newColor = rdr.color;
            newColor.a = Mathf.Lerp(1, 0, (Time.time - startingTime) / duration);
            rdr.color = newColor;
            yield return null;
        }
        rdr.enabled = false;
    }
}
