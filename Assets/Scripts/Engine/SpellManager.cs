﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains all the possible spells in the game
/// </summary>
public class SpellManager : MonoBehaviour
{
    public List<GameObject> spellList;
    public GameObject[] auraPrefabs;
    public GameObject[] auraActivatePrefabs;
    public GameObject[] auraDisablePrefabs;
}