using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    public LayerMask nothing;
    public LayerMask heroLayer;
    public LayerMask heroVisionLayer;
    public LayerMask monsterVisionLayer;
    public LayerMask spellBlockingLayer;
    public LayerMask monsterLayer;
    public LayerMask obstacleLayer;
    public LayerMask blockingLayer;
    public LayerMask highBlockingLayer;
    public LayerMask heroSpells;
    public LayerMask monsterSpells;
    public LayerMask holeLayer;
    public LayerMask heroAndMonsters;

    public static int blockingLayerInt;
    public static int blockingLowInt;
    public static int spellsLayerInt;
    public static int monsterSpellsInt;
    public static int monsterSpellCollidingWithSpellsInt;
    public static int heroSpellCollidingWithSpellsInt;
    public static int obstaclesLayerInt;
    public static int monstersAndHeroLayerInt;
    public static int monsterShieldLayerInt;
    public static int heroShieldLayerInt;
    public static int itemLayersInt;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "LayerManager";

        setupLayers();
    }

    void setupLayers()
    {
        blockingLayerInt = LayerMask.NameToLayer("BlockingLayer");
        blockingLowInt = LayerMask.NameToLayer("BlockingLow");
        spellsLayerInt = LayerMask.NameToLayer("Spells");
        monsterSpellsInt = LayerMask.NameToLayer("MonsterSpells");
        monsterSpellCollidingWithSpellsInt = LayerMask.NameToLayer("MonsterSpellCollidingWithSpells");
        heroSpellCollidingWithSpellsInt = LayerMask.NameToLayer("HeroSpellCollidingWithSpells");
        obstaclesLayerInt = LayerMask.NameToLayer("Obstacles");
        monstersAndHeroLayerInt = LayerMask.NameToLayer("MonstersAndHero");
        monsterShieldLayerInt = LayerMask.NameToLayer("MonsterShield");
        heroShieldLayerInt = LayerMask.NameToLayer("HeroShield");
        itemLayersInt = LayerMask.NameToLayer("Item");
    }
}


