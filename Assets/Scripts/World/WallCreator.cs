using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public enum DoorType { Normal, Big };

public class WallCreator : WorldObjectCreator
{
    public WallTemplate wallTemplate;
    [Range(3, 20)]
    public int length;
    public int doorPosition;
    public DoorStatus doorStatus = DoorStatus.Wall;
    public int openSize = 3;
    public bool hasCornerLeft;
    public bool hasCornerRight;
    public DoorType doorType = DoorType.Normal;

    private List<GameObject> toInstantiate;
    private BoxCollider2D colliderLeft;
    private BoxCollider2D colliderRight;
    private BoxCollider2D colliderCenter;

    private int currentLength;
    private int currentDoorPosition;
    private bool currentHasCornerLeft;
    private bool currentHasCornerRight;
    private int currentOpenSize;
    private DoorStatus currentDoorStatus;
    [HideInInspector]
    public Room parentRoom;
    [HideInInspector]
    public Door door;

    private HashSet<Line2D> gizmoPoints;
    public struct Line2D
    {
        public Vector3 start;
        public Vector3 end;
    }

    public enum DoorStatus { Wall, Door, Open };

    void OnEnable()
    {
        refreshContents();
        gizmoPoints = new HashSet<Line2D>();
        createColliders();
    }

    private void createColliders()
    {
        if (colliderLeft == null)
            colliderLeft = gameObject.AddComponent<BoxCollider2D>();
        if (colliderRight == null)
            colliderRight = gameObject.AddComponent<BoxCollider2D>();
        if (colliderCenter == null)
            colliderCenter = gameObject.AddComponent<BoxCollider2D>();
    }

    public override void refreshContents()
    {
        if (!parentRoom)
            parentRoom = GetComponentInParent<Room>();
        if (!hasChanged())
            return;
        destroyContents();
        if (toInstantiate == null)
            toInstantiate = new List<GameObject>();

        Quaternion savedLocalRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        doorPosition = Mathf.Clamp(doorPosition, 0, length - 3);
        openSize = Mathf.Clamp(openSize, 3, length - 4);
        door = null;

        if (hasCornerLeft)
            instantiateContent(new Vector3(-1f, 0, 0), wallTemplate.exteriorCornerPrefabsNorthWest);
        for (int i = 0; i < length; i++)
        {
            int pos = i;
            toInstantiate.Clear();
            switch (doorStatus)
            {
                case DoorStatus.Wall:
                    toInstantiate.Add(wallTemplate.wall);
                    break;
                case DoorStatus.Door:
                    if (i == doorPosition)
                    {
                        pos = i + 1;
                        i += 2;
                        door = instantiateContent(new Vector3(pos, 0, 0), wallTemplate.getDoor(doorType)).GetComponent<Door>();
                    }
                    else
                        toInstantiate.Add(wallTemplate.wall);
                    break;
                case DoorStatus.Open:
                    if (i == doorPosition)
                    {
                        MeshCreator newMesh = Instantiate<MeshCreator>(wallTemplate.meshCreator);
                        newMesh.size.x = openSize;
                        newMesh.size.y = 2;
                        newMesh.transform.SetParent(transform);
                        newMesh.meshType = MeshCreator.MeshType.Floor;
                        newMesh.tag = "Floor";
                        newMesh.transform.localPosition = new Vector3(i-0.5f, -0.5f, 0);
                        i += (openSize-1);
                    }
                    else if (i == doorPosition - 2)
                    {
                        toInstantiate.Add(Utils.pickRandom(wallTemplate.interiorCornerPrefabsSouthEast));
                        pos = i + 1;
                        i++;
                    }
                    else if (i == doorPosition + 3)
                    {
                        toInstantiate.Add(Utils.pickRandom(wallTemplate.interiorCornerPrefabsSouthWest));
                        pos = i;
                        i++;
                    }
                    else
                        toInstantiate.Add(wallTemplate.wall);
                    break;
            }

            if (toInstantiate.Count == 0)
                continue;

            foreach (GameObject newItem in toInstantiate)
                instantiateContent(new Vector3(pos, 0, 0), newItem);
        }
        if (hasCornerRight)     // Instantiate the corner
            instantiateContent(new Vector3(length, 0, 0), wallTemplate.exteriorCornerPrefabsNorthEast);

        createColliders();
        adjustColliders();

        transform.rotation = savedLocalRotation;
        currentLength = length;
        currentDoorPosition = doorPosition;
        currentDoorStatus = doorStatus;
        currentHasCornerLeft = hasCornerLeft;
        currentHasCornerRight = hasCornerRight;
        currentOpenSize = openSize;
    }

    private void adjustColliders()
    {
        if (hasCornerLeft)
        {
            colliderLeft.size = new Vector2(doorPosition + 2, 2);
            colliderLeft.offset = new Vector2(colliderLeft.size.x / 2 - 0.5f - 2f, 0.5f);
        }
        else
        {
            colliderLeft.size = new Vector2(doorPosition, 2);
            colliderLeft.offset = new Vector2(colliderLeft.size.x / 2 - 0.5f, 0.5f);
        }

        if (hasCornerRight)
        {
            colliderRight.size = new Vector2(length - (doorPosition + 3) + 2, 2);
            colliderRight.offset = new Vector2(doorPosition + 3 + (colliderRight.size.x / 2) - 0.5f, 0.5f);
        }
        else
        {
            colliderRight.size = new Vector2(length - (doorPosition + 3), 2);
            colliderRight.offset = new Vector2(doorPosition + 3 + (colliderRight.size.x / 2) - 0.5f, 0.5f);
        }
        
        colliderCenter.offset = new Vector2(doorPosition + 3 / 2, 0.5f);
        colliderCenter.size = new Vector2(3f, 2f);

        if (doorStatus == DoorStatus.Wall)
            colliderCenter.enabled = true;
        else
            colliderCenter.enabled = false;
    }

    public Vector3 getNextRoomPosition(int distanceBetweenRoom)
    {
        Vector3 nextRoomPosition = transform.position + transform.rotation* new Vector3(doorPosition + 1, 0, 0);
        nextRoomPosition += transform.rotation * Vector3.up * distanceBetweenRoom;
        Line2D line;
        line.start = transform.position;
        line.end = nextRoomPosition;
        gizmoPoints.Add(line);
        return nextRoomPosition;
    }

    public void OnDrawGizmos()
    {
        if (gizmoPoints == null || (gizmoPoints.Count == 0))
            return;
        foreach (Line2D l in gizmoPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(l.start, l.end);
            Gizmos.DrawSphere(l.start, 0.2f);
        }
    }

    public void openDoor(bool value)
    {
        if (value)
            door.openDoor();
        else
            door.closeDoor();
    }
                            

    private bool hasChanged()
    {
        if (currentLength != length)
            return true;
        if (currentDoorPosition != doorPosition)
            return true;
        if (currentDoorStatus != doorStatus)
            return true;
        if (currentHasCornerLeft != hasCornerLeft)
            return true;
        if (currentHasCornerRight != hasCornerRight)
            return true;
        if (currentOpenSize != openSize)
            return true;

        return false;
    }
}
