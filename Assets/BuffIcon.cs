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

    internal void setIcon(Sprite icon)
    {
        buffIconImage.sprite = icon;
    }
}
