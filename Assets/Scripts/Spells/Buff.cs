using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Buff
{
    public string name;
    public string description;
    public Sprite icon;
    public bool timedBuff = true;
    public float timeLeft;
    public float damageReduction;
}