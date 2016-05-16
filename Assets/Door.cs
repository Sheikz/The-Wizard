using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
    public GameObject door;
    public GameObject doorShadow;
    public Collider2D doorCollider;
    public float openingDuration = 2f;
    public float closingDuration = 2f;
    public bool open = true;

    private bool isOpen = true;
    private bool isSwitching = false;
    private SpriteRenderer[] doorRenderers;

    void Awake()
    {
        doorRenderers = door.transform.GetComponentsInChildren<SpriteRenderer>();
        door.SetActive(false);
    }

    private void showDoor(bool value)
    {
        door.SetActive(true);
        if (doorRenderers == null || doorRenderers.Length == 0)
            doorRenderers = door.transform.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer rdr in doorRenderers)
        {
            rdr.enabled = value;
        }
    }

    [ExecuteInEditMode]
    public void refresh()
    {
        if (open != isOpen)
        {
            if (open)
            {
                openDoor();
            } 
            else
            {
                closeDoor();
            }
        }
    }

    public void openDoor()
    {
        if (isSwitching)
        {
            open = isOpen;
            return;
        }
        if (!isOpen)
        {
            isOpen = true;
            doorCollider.enabled = false;
            if (GameManager.instance)
                GameManager.instance.map.gridMap.getTile(transform.position.x, transform.position.y).type = TileType.Floor;
            if (!Application.isPlaying)
            {
                showDoor(true);
                door.transform.localPosition = new Vector3(0, 1, 0);
                if (doorShadow)
                    doorShadow.SetActive(true);
            }
            else
                StartCoroutine(openingRoutine(openingDuration));
        }
    }

    public void closeDoor()
    {
        if (isSwitching)
        {
            open = isOpen;
            return;
        }
        if (isOpen)
        {
            isOpen = false;
            doorCollider.enabled = true;
            if (GameManager.instance)
                GameManager.instance.map.gridMap.getTile(transform.position.x, transform.position.y).type = TileType.Wall;
            if (!Application.isPlaying)
            {
                showDoor(true);
                door.transform.localPosition = new Vector3(0, 0, 0);
                if (doorShadow)
                    doorShadow.SetActive(false);
            }
            else
                StartCoroutine(closingRoutine(closingDuration));
        }
    }

    private IEnumerator openingRoutine(float duration)
    {
        isSwitching = true;
        if (doorShadow)
            doorShadow.SetActive(true);
        showDoor(true);
        float startingTime = Time.time;
        while (Time.time - startingTime <= duration)
        {
            Vector3 tmp = door.transform.localPosition;
            tmp.y = Mathf.Lerp(0f, 1f, (Time.time - startingTime) / duration);
            door.transform.localPosition = tmp;
            yield return null;
        }
        door.transform.localPosition = new Vector3(0, 1, 0);
        showDoor(false);
        isSwitching = false;
        
    }

    private IEnumerator closingRoutine(float duration)
    {
        isSwitching = true;
        if (doorShadow)
            doorShadow.SetActive(true);
        showDoor(true);
        float startingTime = Time.time;
        while (Time.time - startingTime <= duration)
        {
            Vector3 tmp = door.transform.localPosition;
            tmp.y = Mathf.Lerp(1f, 0f, (Time.time - startingTime) / duration);
            door.transform.localPosition = tmp;
            yield return null;
        }
        door.transform.localPosition = new Vector3(0, 0, 0);
        if (doorShadow)
            doorShadow.SetActive(false);
        isSwitching = false;
        
    }
}
