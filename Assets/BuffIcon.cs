using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour 
{
    public Image buffIconImage;
    public Text timeLeftText;

    public Buff buff;
    private HoveringToolTip tooltip;

    void Awake()
    {
        tooltip = GetComponent<HoveringToolTip>();
    }

    internal void setIcon(Buff buff)
    {
        this.buff = buff;
        buffIconImage.sprite = buff.icon;
        tooltip.initialize(buff);

        if (!buff.timedBuff)
            timeLeftText.enabled = false;
    }

    internal void refresh(Buff b)
    {
        enable(true);

        if (buff.icon == b.icon &&
            buff.name == b.name &&
            buff.description == b.description)
        {
            refreshTimeLeft(b.timeLeft);
        }
        else
        {
            setIcon(b);
        }
    }

    public void enable(bool value)
    {
        tooltip.gameObject.SetActive(value);
        buffIconImage.enabled = value;
        if (buff.timedBuff)
            timeLeftText.enabled = value;
    }

    public void refreshTimeLeft(float time)
    {
        timeLeftText.text = Math.Round(time, 1) + "s";
    }
}
