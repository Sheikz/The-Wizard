using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class VisibleRoom : MonoBehaviour
{
    public bool visibleAtStart = false;

    private SpriteRenderer[] spriteRenderers;
    private MeshRenderer[] meshRenderers;
    private bool isRevealed = false;

    private List<VisibleRoom> linkedRooms;

    void Awake()
    {
        linkedRooms = new List<VisibleRoom>();
    }

    void Start()
    {
        setVisible(visibleAtStart);
    }

    public void setVisible(bool value)
    {
        if (transform.parent.GetComponent<Room>() != null)
        {
            spriteRenderers = transform.parent.transform.GetComponentsInChildren<SpriteRenderer>();
            meshRenderers = transform.parent.transform.GetComponentsInChildren<MeshRenderer>();
        }
        else
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }

        if (spriteRenderers == null || meshRenderers == null)
            return;

        if (!value)
        {
            foreach (SpriteRenderer sp in spriteRenderers)
            {
                if (!sp)
                {
                    Debug.Log("sp no longer there");
                    continue;
                }
                sp.enabled = false;
            }

            foreach (MeshRenderer mr in meshRenderers)
            {
                if (!mr)
                {
                    Debug.Log("sp no longer there");
                    continue;
                }
                mr.enabled = false;
            }
        }
        else if (!isRevealed)
        {
            StartCoroutine(progressiveApparition(1f));
            isRevealed = true;
        }
        foreach (VisibleRoom vr in linkedRooms)
        {
            vr.setVisible(value);
        }
    }

    public void linkTo(VisibleRoom room)
    {
        linkedRooms.Add(room);
    }

    private IEnumerator progressiveApparition(float duration)
    {
        yield return null;  // Yield for one frame to let time for the room to initialize
        float startingTime = Time.time;
        foreach (SpriteRenderer sp in spriteRenderers)
        {
            if (!sp)
                continue;

            sp.enabled = true;
        }
        foreach (MeshRenderer mr in meshRenderers)
        {
            if (!mr)
                continue;
            mr.enabled = true;
        }
        while (Time.time - startingTime < duration)
        {
            foreach (SpriteRenderer sp in spriteRenderers)
            {
                if (!sp)
                    continue;
                Color newColor = sp.color;
                newColor.a = Mathf.Lerp(0, 1, (Time.time - startingTime) / duration);
                sp.color = newColor;
            }
            foreach (MeshRenderer mr in meshRenderers)
            {
                if (!mr)
                    continue;
                Color newColor = mr.material.color;
                newColor.a = Mathf.Lerp(0, 1, (Time.time - startingTime) / duration);
                mr.material.color = newColor;
            }
            yield return null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
            setVisible(true);
    }
}
