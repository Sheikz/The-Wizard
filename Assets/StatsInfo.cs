using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class StatsInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemStats stats;
    private Text text;
    private Tooltip tooltip;
    private int value;
    private float fvalue;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (stats != ItemStats.Power)
            return;

        if (tooltip == null)
        {
            tooltip = Instantiate(UIManager.instance.tooltipPrefab);
            tooltip.transform.SetParent(transform);
            tooltip.transform.position = transform.position + new Vector3(25, -10, 0);
            UIManager.instance.characterWindow.registerTooltip(tooltip);
        }
        tooltip.refresh(stats, value);
        tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (stats != ItemStats.Power)
            return;
        tooltip.gameObject.SetActive(false);
    }

    internal void refresh(int value)
    {
        if (text == null)
            text = GetComponent<Text>();
        this.value = value;
        switch (stats)
        {
            case ItemStats.Power: text.text = "Power : <color=magenta>" + value + "</color>";
                break;
            case ItemStats.HP:
                text.text = "HP : <color=green>" + value + "</color>";
                break;
            case ItemStats.MoveSpeed:
                text.text = "Move Speed : <color=orange>" + value + "</color>";
                break;
            case ItemStats.Gold:
                text.text = "Gold : <color=yellow>" + value + "</color>";
                break;
        }
    }

    internal void refresh(float value)
    {
        if (text == null)
            text = GetComponent<Text>();
        this.fvalue = value;
        switch (stats)
        {
            case ItemStats.Power:
                text.text = "Power : <color=magenta>" + fvalue + "</color>";
                break;
            case ItemStats.HP:
                text.text = "HP : <color=green>" + fvalue + "</color>";
                break;
            case ItemStats.MoveSpeed:
                text.text = "Move Speed : <color=orange>" + fvalue + "</color>";
                break;
            case ItemStats.Gold:
                text.text = "Gold : <color=yellow>" + fvalue + "</color>";
                break;
        }
    }
}
