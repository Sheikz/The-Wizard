using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SummonSpell : MonoBehaviour 
{
    public GameObject[] NPCToSummon;

    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    public void summon()
    {
        foreach (GameObject npc in NPCToSummon)
        {
            GameObject newNPC = Instantiate(npc);
            newNPC.transform.position = transform.position;
            newNPC.transform.rotation = Quaternion.identity;
            newNPC.transform.SetParent(GameManager.instance.map.monsterHolder);
            newNPC.layer = spell.emitter.gameObject.layer;
            if (spell.emitter.CompareTag("Hero") || spell.emitter.CompareTag("HeroCompanion"))
            {
                newNPC.tag = "HeroCompanion";
                VisibleUnit unit = newNPC.gameObject.GetComponent<VisibleUnit>();
                if (unit)
                    unit.enabled = false;
            }

            NPCController controller = newNPC.GetComponent<NPCController>();
            if (controller)
                controller.initialize(GameManager.instance.map);

            ExperienceHolder xp = newNPC.GetComponent<ExperienceHolder>();
            if (xp)
                xp.shouldGiveExp = false;

            ItemHolder item = newNPC.GetComponent<ItemHolder>();
            if (item)
                item.shouldDropItems = false;
        }
    }
}
