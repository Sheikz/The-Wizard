using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public Pillar pillar;
    public Pillar pillarWithTorch;
    public Texture2D[] dungeonTileSets;
    public List<ItemWithDropChance> monsters;
    public List<ItemWithDropChance> bosses;
    public List<ItemWithDropChance> furnitures;
    public GameObject wallRumbles;
    public GameObject windowLight;
    public Sprite[] skullDeco;
    public Sprite[] swordDeco;
    public Sprite[] shieldDeco;
    public Sprite[] bannerDecoTop;
    public Sprite[] bannerDecoBottom;
    public Sprite[] rumbleSprites;
    public Sprite[] doorLeft;
    public Sprite[] doorTopLeft;
    public Sprite[] doorRight;
    public Sprite[] doorTopRight;
    public Sprite[] floor;
    public Sprite[] bigDoorLeft;
    public Sprite[] bigDoorTopLeft;
    public Sprite[] bigDoorRight;
    public Sprite[] bigDoorTopRight;
    public Sprite[] stairsTopLeft;
    public Sprite[] stairsTopRight;
    public Sprite[] stairsBottomLeft;
    public Sprite[] stairsBottomRight;
    public Sprite[] deco1Side;
    public Sprite[] deco1InteriorCorner;
    public Sprite[] deco1ExteriorCorner;
    public Sprite[] wallTop;
    public Sprite[] wallBottom;
    public Sprite spikesUp;
    public Sprite spikesDown;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "WorldManager";
    }

    void OnEnable()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "WorldManager";
    }

    internal List<NPCController> getMonsters(MonsterEvent monsterEvent)
    {
        List<NPCController> result = new List<NPCController>();
        int monsterNumbers = 0;
        switch (monsterEvent.roomSize)
        {
            case RoomSize.Small: monsterNumbers = 2; break;
            case RoomSize.Medium: monsterNumbers = 3; break;
            case RoomSize.Large: monsterNumbers = 4; break;
            case RoomSize.UltraLarge: monsterNumbers = 8; break;
        }
        List<ItemWithDropChance> duplicatedList = new List<ItemWithDropChance>(monsters);
        for (int i= 0; i < monsterNumbers; i++)
        {
            ItemWithDropChance chosenItem = ItemWithDropChance.getItem(duplicatedList);
            duplicatedList.Remove(chosenItem);
            result.Add(chosenItem.item.GetComponent<NPCController>());
        }
        return result;
    }

    internal Texture2D getDungeonTileSet()
    {
        if (!GameManager.instance || !GameManager.instance.map)
            return dungeonTileSets[0];

        return dungeonTileSets[GameManager.instance.map.mapTheme];
    }

    internal int getDungeonTheme()
    {
        if (GameManager.instance && GameManager.instance.map)
            return GameManager.instance.map.mapTheme;
        else
            return UnityEngine.Random.Range(0, 3);
    }
}
