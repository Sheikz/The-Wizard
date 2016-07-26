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

    public int blockingLayerInt;
    public int blockingLowInt;
    public int spellsLayerInt;
    public int monsterSpellsInt;
    public int monsterSpellCollidingWithSpellsInt;
    public int heroSpellCollidingWithSpellsInt;
    public int obstaclesLayerInt;
    public int monstersAndHeroLayerInt;
    public int monsterShieldLayerInt;
    public int heroShieldLayerInt;
    public int itemLayersInt;

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


