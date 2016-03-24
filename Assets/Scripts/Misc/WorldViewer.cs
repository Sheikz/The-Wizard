using UnityEngine;
using System.Collections.Generic;

public class WorldViewer : MonoBehaviour
{
    public float visionDistance = 20f;
    private Collider2D[] closeMonsters;

    void FixedUpdate ()
    {
        closeMonsters = Physics2D.OverlapCircleAll(transform.position, visionDistance, GameManager.instance.layerManager.monsterLayer);
        foreach (Collider2D collider in closeMonsters)
        {
            GameObject monster = collider.gameObject;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, monster.transform.position, GameManager.instance.layerManager.heroVisionLayer);
            if (!hit)
            {
                VisibleUnit unit = monster.GetComponent<VisibleUnit>();
                if (unit)
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
