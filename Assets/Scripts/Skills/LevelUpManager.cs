using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public List<ItemWithDropChance> possibleSkillList;  // All the possible skills

    private GameObject hero;
    private Image[] skillIcons;
    private GameObject[] skillChoices;     // The 3 skills that presented to the player at levelup
    private GameObject levelUpText;
    private int levelsToSpend = 0;         // The number of level ups not yet spent

    void Start()
    {
        hero = GameManager.instance.hero.gameObject;
        setupSkillIcons();
        skillChoices = new GameObject[3];
        foreach (ItemWithDropChance skill in possibleSkillList)
        {
            skill.item.GetComponent<Skill>().initialize();
        }
    }

    public void setupSkillIcons()
    {
        levelUpText = transform.Find("LevelUpText").gameObject;

        skillIcons = new Image[3];
        skillIcons[0] = transform.Find("Skill1").GetComponent<Image>();
        skillIcons[1] = transform.Find("Skill2").GetComponent<Image>();
        skillIcons[2] = transform.Find("Skill3").GetComponent<Image>();

        setIconsActive(false);
    }

    public void setIconsActive(bool a)
    {
        for (int i=0; i < skillIcons.Length; i ++)
        {
            skillIcons[i].gameObject.SetActive(a);
        }
        levelUpText.SetActive(a);
    }

    /// <summary>
    ///  Coming from the hero where there is a levelup. Display the interface
    /// </summary>
    public void levelUp()
    {
        if (levelsToSpend == 0)
            generateIcons();
        levelsToSpend++;
    }

    /// <summary>
    /// Display the icons for a level up to happen
    /// </summary>
    public void generateIcons()
    {
        List<ItemWithDropChance> remainingSkills = new List<ItemWithDropChance>();
        foreach (ItemWithDropChance skill in possibleSkillList)
        {
            if (skill.item.GetComponent<Skill>().canBeChosen())
                remainingSkills.Add(skill);
        }

        setIconsActive(true);
        for (int i = 0; i < 3; i++)
        {
            ItemWithDropChance chosenSkill = Utils.getObjectWithProbability(remainingSkills);
            skillChoices[i] = chosenSkill.item;
            if (skillChoices[i] == null)    // No more skills to pick from
                return;

            remainingSkills.Remove(chosenSkill);
            skillChoices[i].GetComponent<Skill>().initializeSkill();
            skillIcons[i].sprite = skillChoices[i].GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void chooseSkill(int skillNumber)
    {
        skillChoices[skillNumber].GetComponent<Skill>().applySkill(hero);
        setIconsActive(false);
        levelsToSpend--;
        if (levelsToSpend > 0)  // Still levels to spend
            generateIcons();
        else if (levelsToSpend < 0)
        {
            Debug.Log("Should not happen. Investigate");
            levelsToSpend = 0;
        }
    }
}
