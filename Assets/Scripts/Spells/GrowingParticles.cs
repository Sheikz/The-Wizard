using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GrowingParticles : MonoBehaviour 
{
    public float timeToReachFullSize = 2;

    void Start()
    {
        ParticleSystem[]  partSystems = GetComponentsInChildren<ParticleSystem>();
        for (int i=0; i < partSystems.Length; i++)
        {
            StartCoroutine(growParticleSystemOverTime(partSystems[i]));
        }
    }

    private IEnumerator growParticleSystemOverTime(ParticleSystem particleSystem)
    {
        float startSize = particleSystem.startSize;
        float startTime = Time.time;
        while (Time.time - startTime < timeToReachFullSize)
        {
            particleSystem.startSize = Mathf.Lerp(0, startSize, (Time.time - startTime));
            yield return new WaitForFixedUpdate();
        }
        particleSystem.startSize = startSize;
    }
}
