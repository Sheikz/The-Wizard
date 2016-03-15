using UnityEngine;
using System.Collections;

public class WaterShield : ShieldSpell 
{
	// Use this for initialization
	new void Start () 
	{
        base.Start();
        if (duration > 0)
		    StartCoroutine(destroyAfterSeconds (duration));
	}
}
