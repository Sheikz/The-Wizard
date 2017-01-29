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
        var main = ps.main;
        main.startSize = 3f;
        GetComponent<ParticleSystemRenderer>().renderMode = ParticleSystemRenderMode.Billboard;
        GetComponent<ParticleSystemRenderer>().sortingLayerName = "SpritesPreCharacter";
    }

    void Start()
    {
        ps.Stop();
        var main = ps.main;
        main.startSize = 3f;
        ps.Play();
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
