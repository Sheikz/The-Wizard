using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FadingObject : MonoBehaviour 
{
    public float fadingTime;

	void Start () 
	{
	    foreach (SpriteRenderer rdr in GetComponentsInChildren<SpriteRenderer>())
        {
            StartCoroutine(fadingRoutine(rdr));
        }
	}

    private IEnumerator fadingRoutine(SpriteRenderer rdr)
    {
        float startTime = Time.time;
        Color col = rdr.color;
        while (Time.time - startTime < fadingTime)
        {
            Color newColor = rdr.color;
            newColor.a = Mathf.Lerp(1, 0, (Time.time - startTime) / fadingTime);
            rdr.color = newColor;
            yield return new WaitForFixedUpdate();
        }
        col.a = 0;
        if (gameObject)
            Destroy(gameObject);
    }
}
