using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Tooltip tooltip;
    public Skill containedSkill;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip == null)
        {
            tooltip = Instantiate(UIManager.instance.tooltipPrefab);
            tooltip.transform.SetParent(transform);
            tooltip.transform.position = transform.position + new Vector3(25, -10, 0);
        }
        tooltip.refresh(containedSkill);
        tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }

    internal void activateToolTip(bool v)
    {
        if (tooltip)
            tooltip.gameObject.SetActive(v);
    }
}
