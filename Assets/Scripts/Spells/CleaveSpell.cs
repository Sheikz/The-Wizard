using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CleaveSpell : MovingSpell 
{
    void FixedUpdate()
    {
        checkIfAlive();
    }
}
