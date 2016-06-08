using UnityEngine;
using System.Collections;

public class RoomEdges : MonoBehaviour
{
    private Room room;
    void Awake()
    {
        room = GetComponentInParent<Room>();
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.isTrigger = true;
        }
    }

    void Start()
    {
        if (transform.localPosition != Vector3.zero)
        {
            Debug.LogWarning(gameObject.name +": room edges local position not equal to origin!");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (!player)
            return;

        if (room)
            room.playerEnteredRoom(player);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (!player)
            return;

        if (room)
            room.playerExitedRoom(player);
    }
}
