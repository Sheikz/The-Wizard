using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Skill : MonoBehaviour
{
    protected SpriteRenderer spr;

    public abstract void applySkill(GameObject hero);
    public virtual void initializeSkill()
    {
        spr = GetComponent<SpriteRenderer>();
        return;
    }

    public virtual bool canBeChosen()
    {
        return true;
    }

    public virtual void initialize()
    {
        return;
    }
}
