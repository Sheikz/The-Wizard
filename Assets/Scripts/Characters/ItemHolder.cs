using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class ItemHolder : MonoBehaviour
{
    public List<ItemWithDropChance> items;
    public bool shouldDropItems = true;

	void OnDestroy()
	{
        if (GameManager.instance.isShuttingDown)    // Avoid looting the items after the game is finished as they stay in the scene
            return;
        
        if (shouldDropItems)
            dropItem(Utils.getObjectWithProbability(items).item);
	}

    private void dropItem(GameObject itemToDrop)
    {
        if (itemToDrop == null)
            return;
        Item newItem = (Instantiate(itemToDrop, transform.position, Quaternion.identity) as GameObject).GetComponent<Item>();
        newItem.transform.SetParent(GameManager.instance.map.transform);
        CharacterStats stats = GetComponent<CharacterStats>();
        if (stats)
            newItem.level = stats.level;
    }

}
