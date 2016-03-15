using UnityEngine;
using System.Collections;
using System;

public class DecoCreator : WorldObjectCreator
{
    [System.Serializable]
    public struct DecoSet
    {
        public GameObject side;
        public GameObject corner;
        public GameObject center;
    }
    public DecoSet[] decoSets;
    public int width;
    public int height;
    public int index;
    public bool hasCollider;

    public override void refreshContents()
    {
        index = Math.Max(index % decoSets.Length, 0); // Clamp the value to stay within the index
        destroyContents();

        Quaternion savedLocalRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        GameObject toInstantiate = null;
        Quaternion rotation = Quaternion.identity;
        Vector3 position = Vector3.zero; 

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0)
                {
                    toInstantiate = decoSets[index].side;   // Left side
                    rotation = Quaternion.Euler(0, 0, 90);
                    if (y == 0)             // Bottom left corner
                    {
                        toInstantiate = decoSets[index].corner;
                        rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else if (y == height - 1)   // Top left corner
                    {
                        toInstantiate = decoSets[index].corner;
                        rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                else if (x == width-1)  // Right side
                {
                    toInstantiate = decoSets[index].side;
                    rotation = Quaternion.Euler(0, 0, -90);
                    if (y == 0)         // Bottom right corner
                    {
                        toInstantiate = decoSets[index].corner;
                        rotation = Quaternion.Euler(0, 0, 180);

                    }
                    else if (y == height - 1)   // Top Right corner
                    {
                        toInstantiate = decoSets[index].corner;
                        rotation = Quaternion.Euler(0, 0, -90);
                    }
                }
                else if (y == 0)    // Bottom side
                {
                    toInstantiate = decoSets[index].side;
                    rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (y == height -1) // Top side
                {
                    toInstantiate = decoSets[index].side;
                    rotation = Quaternion.Euler(0, 0, 0);
                }
                else // Center
                {
                    toInstantiate = decoSets[index].center;
                }
                position = new Vector3(x, y, 0);
                if (toInstantiate != null)
                    instantiateContent(position, rotation, toInstantiate);
            }
        }

        if (hasCollider)
        {
            BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(width, height);
            bc.offset = new Vector2(width / 2, height / 2);
            bc.isTrigger = true;
        }

        transform.rotation = savedLocalRotation;
    }
}
