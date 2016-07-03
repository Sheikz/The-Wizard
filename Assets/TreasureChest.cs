using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureChest : MonoBehaviour
{
    public GameObject openChestPrefab;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != GameManager.instance.hero.gameObject)
            return;

        open();
    }

    void open()
    {
        foreach (GameObject treasureContent in ItemManager.instance.getTreasureContents())
        {
            Instantiate(treasureContent, transform.position, Quaternion.identity);
        }
        SoundManager.instance.playSound("ClickEsc");
        GameObject openChest = Instantiate(openChestPrefab, transform.position, Quaternion.identity) as GameObject;
        openChest.transform.SetParent(GameManager.instance.map.furnitureHolder);
        Destroy(transform.parent.gameObject);
    }
}
