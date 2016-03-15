using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ParticleAnimation : MonoBehaviour
{
    public float fadeLightsDuration = 0.1f;
    public float shapeRadiusModifier;

    void Start()
    {
        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        if (shapeRadiusModifier <= 0f)
            return;

        foreach (ParticleSystem ps in partSystems)
        {
            StartCoroutine(modifyShapeRadius(ps));
        }
    }

    private IEnumerator modifyShapeRadius(ParticleSystem ps)
    {
        ParticleSystem.ShapeModule shape = ps.shape;
        while (ps.shape.radius >= 0)
        {
            shape.radius -= shapeRadiusModifier;
            yield return new WaitForFixedUpdate();
        }
    }

    void FixedUpdate()
    {
        checkIfAlive();
    }

    protected void checkIfAlive()
    {
        if (transform.GetComponentsInChildren<ParticleEmitter>().Length > 0)
            return; // Legacy Particle System can be defined as one-shot and dont need to be destroyed manually

        ParticleSystem[] partSystems = GetComponentsInChildren<ParticleSystem>();
        if (partSystems.Length == 0)
        {
            StartCoroutine(fadeLights(fadeLightsDuration));
        }

        foreach (ParticleSystem ps in partSystems)
        {
            if (!ps.IsAlive())
                Destroy(ps.gameObject);
        }
    }

    IEnumerator fadeLights(float duration)
    {
        float startingTime = Time.time;
        Light[] lights = GetComponentsInChildren<Light>();
        if (lights.Length > 0)
        {
            float startingIntensity = lights[0].intensity;

            while ((Time.time - startingTime) * Time.timeScale < duration)
            {
                foreach (Light light in lights)
                {
                    if (light == null)
                        continue;
                    light.intensity = Mathf.Lerp(startingIntensity, 0, (Time.time - startingTime) * Time.timeScale / duration);
                }
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
