using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class ItemHolder : MonoBehaviour
{
	public bool shouldDropItems = true;

	public void die()
	{
		if (GameManager.instance.isShuttingDown)    // Avoid looting the items after the game is finished as they stay in the scene
			return;

        if (shouldDropItems)
        {
            foreach (ItemWithDropChance item in ItemManager.instance.monsterItems)
            {
                if (Random.Range(0f, 1f) <= item.lootChance)
                    dropItem(item.item);
            }
        }
	}

	private void dropItem(GameObject itemToDrop)
	{
		if (itemToDrop == null)
			return;
		Item newItem = (Instantiate(itemToDrop, transform.position, Quaternion.identity) as GameObject).GetComponent<Item>();
		newItem.transform.SetParent(GameManager.instance.map.transform);
		CharacterStats stats = GetComponent<CharacterStats>();
		if (stats)
			newItem.initialize(stats);
	}

}
