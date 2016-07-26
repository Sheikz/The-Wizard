using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpikesTrap : Trap 
{
    public bool up = false;
    public float changeFrequency = 5;

    private SpriteRenderer rdr;
    private Collider2D col;

    void Awake()
    {
        rdr = GetComponent<SpriteRenderer>();
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
        rdr.sprite = up ? WorldManager.instance.spikesUp : WorldManager.instance.spikesDown;
        col.enabled = up;
    }
}
