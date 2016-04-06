using UnityEngine;
using System.Collections;

public class Aura : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        ps.startSize = 3f;
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
