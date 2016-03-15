using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(StaticSpell))]
public class GrowingStaticSpell : MonoBehaviour
{
    public float startingRadius = 0.5f;
    [Tooltip("The additional radius per second")]
    public float growingRate = 0.5f;

    private float radiusToParticleSize = 3.2f;
    private StaticSpell spell;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        spell = GetComponent<StaticSpell>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

	void Start()
    {
        if (growingRate <= 0)
            return;

        StartCoroutine(growingRoutine());
        foreach (ParticleSystem partSystem in GetComponentsInChildren<ParticleSystem>())
        {
            partSystem.startSize = startingRadius * radiusToParticleSize;
            StartCoroutine(growingRoutine(partSystem));
        }
	}

    private IEnumerator growingRoutine(ParticleSystem partSystem)
    {
        float startingTime = Time.realtimeSinceStartup;
        while ((Time.realtimeSinceStartup - startingTime)*Time.timeScale < spell.duration)
        {
            yield return new WaitForSeconds(0.1f);
            partSystem.startSize += growingRate * 0.1f * radiusToParticleSize;
            yield return null;
        }
        //partSystem.loop = false;
    }

    private IEnumerator growingRoutine()
    {
        float startingTime = Time.realtimeSinceStartup;
        circleCollider.radius = startingRadius;
        while ((Time.realtimeSinceStartup - startingTime) * Time.timeScale < spell.duration)
        {
            yield return new WaitForSeconds(0.1f);
            circleCollider.radius += growingRate * 0.1f;
            yield return null;
        }
    }
}
