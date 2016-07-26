using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EquipableItem : Item 
{
    [HideInInspector]
	public EquipableItemStats itemStats;
	private SpriteRenderer spriteRenderer;
    private TextOnMouseOver mOver;

	public override void isPickedUpBy (Inventory looter)
	{
		looter.addItem (itemStats);
        SoundManager.instance.playSound("GetTreasure");
        Destroy(gameObject);
	}

	new void Awake()
	{
        base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.enabled = false;
    }

	new void Start()
	{
        base.Start();
		itemStats = new EquipableItemStats(level);
		spriteRenderer.sprite = itemStats.sprite;
		spriteRenderer.enabled = true;
        instantiateAura();
	}

    void instantiateAura()
    {
        GameObject auraToInstantiate = ItemManager.instance.itemAuras[(int)itemStats.rarity];
        if (!auraToInstantiate)
            return;

        (Instantiate(auraToInstantiate, transform.position, Quaternion.identity) as GameObject).transform.SetParent(transform);
    }
}
