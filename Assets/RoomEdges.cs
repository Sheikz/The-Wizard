using UnityEngine;
using System.Collections;

public class RoomEdges : MonoBehaviour
{
    private Room room;
    void Awake()
    {
        room = GetComponentInParent<Room>();
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
