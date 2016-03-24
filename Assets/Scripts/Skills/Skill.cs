using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Skill : MonoBehaviour
{
    public string skillName;
    public string description;

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

    public virtual string getDescription()
    {
        string result = "";
        string name = skillName;
        if (name == "")
            name = "#Undefined#";

        result += "<size=18>" + name + "</size>" + "\n";
        result += description;

        return result;
    }
}
