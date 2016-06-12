using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public enum ToolTipType { Spell, Item, Buff, Manual };

public class HoveringToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;

    private ToolTipType tooltipType;
    private Tooltip tooltip;
    private SpellCaster hero;
    private SpellController containedSpell;
    private Buff buff;

    void Awake()
    {
        if (text != "")
            tooltipType = ToolTipType.Manual;
    }

    public void initialize(SpellCaster hero, SpellController spell)
    {
        this.hero = hero;
        this.containedSpell = spell;
        tooltipType = ToolTipType.Spell;
    }

    public void initialize(Buff buff)
    {
        this.buff = buff;
        tooltipType = ToolTipType.Buff;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipType == ToolTipType.Spell && !containedSpell)
            return;

        if (tooltip)
        {
            tooltip.gameObject.SetActive(true);
            refreshToolTip();
        }
        else
        {
            tooltip = Instantiate(UIManager.instance.tooltipPrefab);
            tooltip.transform.SetParent(transform);
            tooltip.transform.position = transform.position + new Vector3(25, -10, 0);
            refreshToolTip();
        }
    }

    private void refreshToolTip()
    {
        if (text != "") // Forcing text to this
            tooltip.refresh(text);

        switch (tooltipType)
        {
            case ToolTipType.Spell: tooltip.refresh(hero, containedSpell); break;
            case ToolTipType.Buff: tooltip.refresh(buff); break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}
