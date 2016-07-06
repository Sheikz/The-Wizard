using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpikesTrap : Trap 
{
    public bool up = false;
    public float changeFrequency = 5;

    private SpriteRenderer withoutSpikes;
    private SpriteRenderer withSpikes;
    private Collider2D col;

    void Awake()
    {
        withSpikes = transform.Find("Spikes").GetComponent<SpriteRenderer>();
        withoutSpikes = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        refresh();
        StartCoroutine(alternateUpAndDown());
    }

    private IEnumerator alternateUpAndDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeFrequency);
            up = !up;
            refresh();
        }
    }

    public void refresh()
    {
        withSpikes.enabled = up;
        withoutSpikes.enabled = !up;
        col.enabled = up;
    }
}
