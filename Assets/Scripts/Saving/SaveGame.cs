using System;

[Serializable]
public class SaveGame
{
    private struct SpellIdentifier
    {
        public MagicElement element;
        public SpellType type;
        public SpellIdentifier(MagicElement n, SpellType t)
        {
            element = n;
            type = t;
        }
    }

    private int heroLevel;
    private int[,] spellsLevel;
    private int[] pointsToAllocate;
    private SpellIdentifier[] equippedSpells;
    public SpellCaster caster;

    public void saveHeroStats(SpellCaster hero)
    {
        saveSpellCaster(hero);
        saveStats(hero.GetComponent<PlayerStats>());
        saveInventory(hero.GetComponent<Inventory>());
    }

    public void saveSpellCaster(SpellCaster hero)
    {
        equippedSpells = new SpellIdentifier[hero.spellList.Length];
        for (int i = 0; i < hero.spellList.Length; i++)
        {
            equippedSpells[i] = new SpellIdentifier(hero.spellList[i].magicElement, hero.spellList[i].spellType);
        }
    }

    public void saveHero(SpellCaster caster)
    {
        this.caster = caster;
    }

    private void saveStats(PlayerStats playerStats)
    {
        heroLevel = playerStats.level;
        return;
    }

    private void saveInventory(Inventory inventory)
    {
        return;
    }

    
}
