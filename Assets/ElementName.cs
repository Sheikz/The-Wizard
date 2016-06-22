using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ElementName : MonoBehaviour 
{
    public MagicElement element;
    private PlayerStats heroStats;
    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

	public void refresh()
    {
        if (!heroStats)
            heroStats = GameManager.instance.hero.GetComponent<PlayerStats>();
        if (!heroStats)
            return;

        text.enabled = heroStats.elementUnlocked[(int)element];
    }
}
