using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AutoPilot))]
public class FollowObjects : MonoBehaviour
{
    public string followingTag;
    public float followingRadius;
    private LayerMask spellLayer;
    private AutoPilot autoPilot;

    void Awake()
    {
        autoPilot = GetComponent<AutoPilot>();
    }

    void Start()
    {
        autoPilot.disableAutoPilot();
        spellLayer = GameManager.instance.layerManager.heroSpells | GameManager.instance.layerManager.monsterSpells;
    }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, followingRadius, spellLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(followingTag))
            {
                autoPilot.lockToObject(hit.transform);
            }
        }
    }
}
