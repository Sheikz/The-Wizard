using UnityEngine;
using System.Collections;

public class Aura : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ps.startSize = 3f;
    }

    void Start()
    {
        ps.startSize = 3f;
        var rotationOverLifeTime = ps.rotationOverLifetime;
        rotationOverLifeTime.z = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad*45);
    }

    void FixedUpdate()
    {
        checkIfAlive();
    }

    void checkIfAlive()
    {
        if (!ps.IsAlive())
            Destroy(ps.gameObject);
    }
}
