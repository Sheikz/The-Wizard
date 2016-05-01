using UnityEngine;
using System.Collections;

public class Aura : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        if (!ps)
            ps = GetComponent<ParticleSystem>();
    }

    public void initialize()
    {
        ps = GetComponent<ParticleSystem>();
        ps.startSize = 3f;
        GetComponent<ParticleSystemRenderer>().renderMode = ParticleSystemRenderMode.Billboard;
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
