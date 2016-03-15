using UnityEngine;
using System.Collections;
using System;

public class FloorCreator : WorldObjectCreator
{
    public GameObject[] floors;
    public GameObject[] floorsPlain;
    public int width;
    public int height;
    public int typeOfFloor = 0;

    public override void refreshContents()
    {
        createSprites();
    }

    public void createSprites()
    {
        destroyContents();
        Quaternion savedLocalRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (typeOfFloor % 2 == 0)
                    instantiateContent(new Vector3(x, y, 0), floors);
                else
                    instantiateContent(new Vector3(x, y, 0), floorsPlain);
            }
        }

        transform.rotation = savedLocalRotation;
    }
}
