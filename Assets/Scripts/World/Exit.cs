using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == GameManager.instance.hero.gameObject)
            if (other.bounds.Contains(transform.position))
                GameManager.instance.endLevel();
    }
}
