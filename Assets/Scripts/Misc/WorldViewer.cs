using UnityEngine;
using System.Collections.Generic;

public class WorldViewer : MonoBehaviour
{
    public float visionDistance = 20f;
    private Collider2D[] closeUnits;

    void FixedUpdate ()
    {
        closeUnits = Physics2D.OverlapCircleAll(transform.position, visionDistance);
        foreach (Collider2D collider in closeUnits)
        {
            VisibleUnit unit = collider.gameObject.GetComponent<VisibleUnit>();
            if (!unit || unit.isVisible)
                continue;

            RaycastHit2D hit = Physics2D.Linecast(transform.position, unit.transform.position, GameManager.instance.layerManager.heroVisionLayer);
            if (!hit || hit.collider.gameObject == unit.gameObject)
            {
                unit.setVisible();
            }
        }
	}

    void OnDrawGizmos()
    {
        if (closeUnits == null)
            return;

        foreach (Collider2D collider in closeUnits)
        {
            if (!collider)
                continue;
            Gizmos.DrawLine(transform.position, collider.transform.position);
        }

    }
}
