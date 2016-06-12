using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffArea : MonoBehaviour
{
    public Buff buff;

    private List<BuffsReceiver> applyingBuffTo;

    void Awake()
    {
        applyingBuffTo = new List<BuffsReceiver>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        BuffsReceiver bReceiver = collision.GetComponent<BuffsReceiver>();
        if (bReceiver)
        {
            bReceiver.addBuff(buff);
            applyingBuffTo.Add(bReceiver);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        BuffsReceiver bReceiver = collision.GetComponent<BuffsReceiver>();
        if (bReceiver)
        {
            bReceiver.removeBuff(buff);
            applyingBuffTo.Remove(bReceiver);
        }
    }

    void OnDestroy()
    {
        foreach (BuffsReceiver bReceiver in applyingBuffTo)
        {
            bReceiver.removeBuff(buff);
        }
    }
}
