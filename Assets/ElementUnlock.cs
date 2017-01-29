using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ElementUnlock : Item {

    private int element;
    private SpriteRenderer spriteRenderer;

    private PlayerStats stats;

    private string[] elementTexts = {
        "You unlocked Light magic!",
        "You unlocked Air magic!",
        "You unlocked Fire magic!",
        "You unlocked Arcane magic!",
        "You unlocked Ice magic!",
        "You unlocked Earth magic!",
        "You unlocked Dark magic!"
    };

    new void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    new void Start()
    {
        base.Start();
        randomizeElement();
    }

    void randomizeElement()
    {
        stats = GameManager.instance.hero.GetComponent<PlayerStats>();
        // Get indexes of locked elements
        int[] elementsLocked = stats.elementUnlocked.Select((v, i) => new { Index = i, Value = v })
                                    .Where(p => p.Value == false)
                                    .Select(p => p.Index).ToArray();

        // If no elements are locked, everything is already discovered. Destroy itself
        if (elementsLocked.Length == 0)
            Destroy(gameObject);

        element = Utils.pickRandom(elementsLocked);
        setIcon();
    }

    void setIcon()
    {
        spriteRenderer.sprite = UIManager.instance.elementIcons[element];
        spriteRenderer.enabled = true;
    }

    public override void isPickedUpBy(Inventory looter)
    {
        if (looter.gameObject != GameManager.instance.hero.gameObject)
        {
            Destroy(gameObject);
            return;
        }
        stats.elementUnlocked[element] = true;
        InfoPopup.createPopup(UIManager.instance.elementIcons[element], elementTexts[element]);
        Destroy(gameObject);
    }

}
