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
            Debug.LogWarning(room.name +": room edges local position not equal to origin! at "+room.transform.position);
        }
    }
}
