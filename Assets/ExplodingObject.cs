using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodingObject : MonoBehaviour 
{
    public Explosion explosion;
    public float damageRatio;

    public void explode()
    {
        Explosion newExplosion = Instantiate(explosion, transform.position, Quaternion.identity) as Explosion;
        newExplosion.initialize(this);
    }
}
