using UnityEngine;
using System.Collections;
using System;

public class Pillar : MonoBehaviour
{
    private Room room;
    private bool hasTorch = false;

    /// <summary>
    /// Initialize the pillar
    /// </summary>
    /// <param name="hasPillar">indicate if this pillar should be visible</param>
    public void initialize(bool hasPillar, Room parentRoom)
    {
        this.room = parentRoom;
        if (!hasPillar)
        {
            gameObject.SetActive(false);
            return;
        }
        Transform torch = transform.FindChild("Torch");
        if (torch)
            hasTorch = true;

        replaceWithPrefab();
    }

    private void replaceWithPrefab()
    {
        Pillar pillarPrefab;
        if (hasTorch)
            pillarPrefab = WorldManager.instance.pillarWithTorch;
        else
            pillarPrefab = WorldManager.instance.pillar;

        if (pillarPrefab == null)
        {
            Debug.LogError("Could not find pillar prefab");
            return;
        }

        Pillar newPillar = Instantiate(pillarPrefab, transform.position, Quaternion.identity) as Pillar;
        newPillar.transform.SetParent(room.transform);
        Destroy(gameObject);
    }
}
