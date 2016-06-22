using UnityEngine;
using System.Collections.Generic;

public class WorldViewer : MonoBehaviour
{
    public float visionDistance = 20f;
    private Collider2D[] closeMonsters;

    void FixedUpdate ()
    {
        closeMonsters = Physics2D.OverlapCircleAll(transform.position, visionDistance);
        foreach (Collider2D collider in closeMonsters)
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
        if (closeMonsters == null)
            return;

        foreach (Collider2D collider in closeMonsters)
        {
            if (!collider)
                continue;
            Gizmos.DrawLine(transform.position, collider.transform.position);
        }

    }
}
