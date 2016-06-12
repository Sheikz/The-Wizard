using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour 
{
    public string buffName;
    public string description;
    public float timeLeft;

    public Image buffIconImage;
    public Text timeLeftText;

    private Buff buff;
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

    void FixedUpdate()
    {
        if (buff == null)
            return;

        if (!buff.timedBuff)    // Should no show the time left
        {
            if (timeLeftText.enabled)
                timeLeftText.enabled = false;
            return;
        }

        if (!timeLeftText.enabled)
            timeLeftText.enabled = true;

        refreshTimeLeft(buff.timeLeft);
    }

    public void refreshTimeLeft(float time)
    {
        timeLeftText.text = Math.Round(time, 1) + "s";
    }
}
