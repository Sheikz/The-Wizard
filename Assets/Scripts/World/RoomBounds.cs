using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomBounds : MonoBehaviour
{
    private Room room;  // reference to the room
    private BoxCollider2D[] roomBounds;

    private HashSet<PlayerController> playersEntered;   // Player that are currently entering
    private HashSet<PlayerController> playersInside;    // Player that are inside
    private Stack<PlayerController> playersToRemove;

    void Awake()
    {
        playersEntered = new HashSet<PlayerController>();
        playersInside = new HashSet<PlayerController>();
        playersToRemove = new Stack<PlayerController>();
        roomBounds = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D bc in roomBounds)
            bc.isTrigger = true;
        room = GetComponentInParent<Room>();
        gameObject.layer = LayerMask.NameToLayer("OnlyHero");
    }

    void Update()
    {
        Stack<PlayerController> playersToMoveInside = new Stack<PlayerController>();
        if (playersEntered != null)
            foreach (PlayerController player in playersEntered)
            {
                CircleCollider2D col = player.circleCollider;
                foreach (BoxCollider2D box in roomBounds)
                {
                    if (!box.bounds.Contains(col.transform.position))
                        continue;

                    foreach (Vector3 direction in new Vector3[] { Vector3.up, Vector3.right, Vector3.down, Vector3.left})
                    {
                        if (!box.bounds.Contains(col.transform.position + direction * col.radius))
                            return;
                    }
                    playersToMoveInside.Push(player);
                }
            }
            for (int i = 0; i < playersToMoveInside.Count; i++)
            {
                PlayerController playerToMove = playersToMoveInside.Pop();
                playersEntered.Remove(playerToMove);
                playersInside.Add(playerToMove);
                room.playerEnteredRoom(playerToMove);
            }

        if (playersToRemove != null)
            playersToRemove.Clear();
        foreach (PlayerController player in playersInside)   // Reverse order to allow removing while looping
        {
            bool playerInside = false;
            foreach (BoxCollider2D box in roomBounds)
            {
                if (!player)
                    return;
                if (box.bounds.Contains(player.transform.position))
                {
                    playerInside = true;
                    break;
                }
            }
            if (!playerInside)
            {
                playersToRemove.Push(player);
            }
        }
        foreach (PlayerController player in playersToRemove)
        {
            room.playerExitedRoom(player);
            playersInside.Remove(player);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc)
        {
            playersEntered.Remove(pc);
            playersInside.Remove(pc);
            room.playerExitedRoom(pc);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc)
            playersEntered.Add(pc);
    }

}
