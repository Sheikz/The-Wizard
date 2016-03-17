using UnityEngine;
using System.Collections.Generic;

public class Glyph : StaticSpell
{
    public Explosion explosion;

    new void FixedUpdate()
    {
        checkIfAlive();

        for (int i = affectedObjects.Count - 1; i >= 0; i--)
        {
            if (affectedObjects[i] == null)
            {
                affectedObjects.RemoveAt(i);
                continue;
            }
        }
    }
}
