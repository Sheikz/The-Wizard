using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdjustSortingOrder : MonoBehaviour 
{
    public bool aggregateObject = false;

    private SpriteRenderer rdr;
    private AdjustSortingOrder parentSortingOrder;

	void Start()
    {
        rdr = GetComponent<SpriteRenderer>();
        if (transform.parent)
            parentSortingOrder = transform.parent.GetComponentInParent<AdjustSortingOrder>();
        if (!rdr)
            return;

        if (aggregateObject && parentSortingOrder) // There is a parent sorting order
        {
            float sign = Mathf.Sign(parentSortingOrder.transform.position.y - transform.position.y);
            rdr.sortingOrder = -Mathf.RoundToInt((parentSortingOrder.transform.position.y * 10) + sign);
        }
        else
        {
            rdr.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
        }
    }
}
