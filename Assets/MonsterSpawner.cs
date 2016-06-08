using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public bool bossEvent = false;
    public NPCController[] monsterPrefabs;

    private Room room;
    private List<NPCController> spawnedMonsterList;

    void Awake()
    {
        room = GetComponentInParent<Room>();
        spawnedMonsterList = new List<NPCController>();
    }

    public void spawnMonster()
    {
        if (bossEvent)
        {
            GameObject bossToInstantiate = ItemWithDropChance.getItem(WorldManager.instance.bosses).item;
            NPCController newMonster = (Instantiate(bossToInstantiate, transform.position, Quaternion.identity) as GameObject).GetComponent<NPCController>();
            newMonster.transform.parent = room.transform;
            newMonster.initialize(room);
            spawnedMonsterList.Add(newMonster);
            return;
        }
        foreach (NPCController mc in monsterPrefabs)
        {
            NPCController newMonster = Instantiate(mc, transform.position, Quaternion.identity) as NPCController;
            newMonster.transform.parent = room.transform;
            newMonster.initialize(room);
            spawnedMonsterList.Add(newMonster);
        }
    }
    
    /// <summary>
    ///  Clear the monsters without them looting items
    /// </summary>
    public void clearMonsters()
    {
        foreach (NPCController mc in spawnedMonsterList)
        {
            if (mc)
            {
                ItemHolder itemHolder = mc.GetComponent<ItemHolder>();
                if (itemHolder)
                    itemHolder.shouldDropItems = false;

                Destroy(mc.gameObject);
            }
        }
        spawnedMonsterList.Clear();
    }
}
