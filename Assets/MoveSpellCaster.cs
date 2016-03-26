using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SpellController))]
public class MoveSpellCaster : MonoBehaviour
{
    public enum MoveSpellCasterType { Slide, Teleport};
    public MoveSpellCasterType type;
    public Explosion explosion;
    public float timeBeforeTeleport = 0.5f;
    public float cooldown;
    private SpellController spell;

    void Awake()
    {
        spell = GetComponent<SpellController>();
    }

    void Start()
    {
        MovingCharacter character = spell.emitter.GetComponent<MovingCharacter>();
        if (type == MoveSpellCasterType.Slide && character)
            character.isFlying = true;

        if (spell)
        {
            Debug.Log("start");
            spell.emitter.spellList[(int)spell.spellType].moveSpellCaster = this;
        }
    }

    void FixedUpdate()
    {
        if (!spell)
            return;

        if (type == MoveSpellCasterType.Slide)
        {
            spell.emitter.transform.position = transform.position;
            if ((transform.position - spell.target).sqrMagnitude <= 0.05f)
            {
                Explode();
            }
        }
    }

    void Update()
    {
        if (type != MoveSpellCasterType.Teleport)
            return;

        if (InputManager.instance.IsKeyDown((InputManager.Command)spell.spellType))
            TeleportToLocation();

    }

    void Explode()
    {
        if (explosion != null)  
        {
            Explosion newExplosion = Instantiate(explosion).GetComponent<Explosion>();
            newExplosion.transform.position = transform.position;   // if the object explodes, the exposion is created where it was

            newExplosion.initialize(spell);
        }
        MovingCharacter character = spell.emitter.GetComponent<MovingCharacter>();
        if (character)
            character.isFlying = false;

        Destroy(gameObject);
    }

    /// <summary>
    /// Create an explosion at spellcaster and location, then teleport the spellcaster to the location
    /// </summary>
    void TeleportToLocation()
    {
        if (explosion != null)  // Create an explosion at the emitter, and one at the location of the spell
        {
            Explosion newExplosion1 = Instantiate(explosion);
            newExplosion1.transform.position = spell.emitter.transform.position;   // if the object explodes, the exposion is created where it was
            newExplosion1.initialize(spell);

            Explosion newExplosion2 = Instantiate(explosion);
            newExplosion2.transform.position = transform.position;   // if the object explodes, the exposion is created where it was

            newExplosion2.initialize(spell);
        }
        spell.emitter.transform.position = transform.position;
        spell.emitter.startCooldown(spell.spellType, cooldown);
        Destroy(gameObject);
    }

    /// <summary>
    /// Check if we can cast at this position
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="position"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    internal bool canCastSpell(SpellCaster emitter, Vector3 position, Vector3 target)
    {
        if (type == MoveSpellCasterType.Teleport)
            return true;

        RaycastHit2D hit = Physics2D.Linecast(position, target, GameManager.instance.layerManager.blockingLayer);
        if (hit)
        {
            return false;
        }
        return true;
    }
}
