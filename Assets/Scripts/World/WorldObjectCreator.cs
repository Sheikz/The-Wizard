using UnityEngine;
using System.Collections.Generic;

public abstract class WorldObjectCreator : MonoBehaviour
{
    [ExecuteInEditMode]
    public abstract void refreshContents();

    /// <summary>
    /// Instantiate a prefab a the local position specified
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="localPos"></param>
    /// <returns></returns>
    protected GameObject instantiateContent(Vector3 localPos, params GameObject[] prefab)
    {
        if (prefab == null || prefab.Length == 0)
            return null;
        GameObject newContent = Instantiate(Utils.pickRandom(prefab)) as GameObject;
        newContent.transform.SetParent(transform);
        newContent.transform.localPosition = localPos;
        return newContent;
    }

    protected GameObject instantiateContent(Vector3 localPos, Quaternion rotation, params GameObject[] prefab)
    {
        if (prefab == null || prefab.Length == 0)
            return null;
        GameObject newContent = Instantiate(Utils.pickRandom(prefab)) as GameObject;
        newContent.transform.SetParent(transform);
        newContent.transform.localPosition = localPos;
        newContent.transform.rotation = rotation;
        return newContent;
    }

    /// <summary>
    /// Destroy the contents
    /// </summary>
    protected void destroyContents()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        List<Collider2D> toDestroy2 = new List<Collider2D>();
        foreach (Transform child in transform)
            toDestroy.Add(child.gameObject);

        foreach (Collider2D c in gameObject.GetComponents<Collider2D>())
            toDestroy2.Add(c);

        foreach (GameObject td in toDestroy)
            DestroyImmediate(td);

        foreach (Collider2D cd in toDestroy2)
            DestroyImmediate(cd);

    }
}
