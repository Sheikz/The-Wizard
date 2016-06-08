using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffArea : MonoBehaviour
{
    public Buff buff;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        BuffsReceiver bReceiver = collision.GetComponent<BuffsReceiver>();
        if (bReceiver)
        {
            bReceiver.addBuff(buff);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        BuffsReceiver bReceiver = collision.GetComponent<BuffsReceiver>();
        if (bReceiver)
        {
            bReceiver.removeBuff(buff);
        }
    }
}
