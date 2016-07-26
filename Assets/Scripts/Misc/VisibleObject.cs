using UnityEngine;
using System.Collections;

public abstract class VisibleObject : MonoBehaviour
{
    public float progressiveApparitionTime = 0.5f;
    private SpriteRenderer[] spriteRenderers;
    private Light[] lights;
    private FloatingHPBar HPBar;
    private CastingBar castingBar;

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
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            return;

        if (HPBar == null)
            HPBar = GetComponentInChildren<FloatingHPBar>();
        if (HPBar)
            HPBar.setVisible(value);

        if (castingBar == null)
            castingBar = GetComponentInChildren<CastingBar>();
        if (castingBar)
            castingBar.setVisible(value);

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
            StartCoroutine(appearAfterSeconds(progressiveApparitionTime, spr));
        }
    }

    private IEnumerator appearAfterSeconds(float duration, SpriteRenderer rdr)
    {
        rdr.enabled = true;
        float startingTime = Time.time;
        Color newColor;
        while (Time.time - startingTime < duration)
        {
            newColor = rdr.color;
            newColor.a = Mathf.Lerp(0, 1, (Time.time - startingTime) / duration);
            rdr.color = newColor;
            yield return null;
        }
        newColor = rdr.color;
        newColor.a = 1f;
        rdr.color = newColor;
        rdr.enabled = true;
    }

    protected void startProggressiveDisparition()
    {
        foreach (SpriteRenderer spr in spriteRenderers)
        {
            StartCoroutine(disappearAfterSeconds(progressiveApparitionTime, spr));
        }
    }

    private IEnumerator disappearAfterSeconds(float duration, SpriteRenderer rdr)
    {
        float startingTime = Time.time;
        Color newColor;
        while (Time.time - startingTime < duration)
        {
            newColor = rdr.color;
            newColor.a = Mathf.Lerp(1, 0, (Time.time - startingTime) / duration);
            rdr.color = newColor;
            yield return null;
        }
        newColor = rdr.color;
        newColor.a = 0f;
        rdr.color = newColor;
        rdr.enabled = false;
    }
}
