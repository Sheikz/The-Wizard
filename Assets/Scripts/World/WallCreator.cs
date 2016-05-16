using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public enum DoorType { Normal, Big };

public class WallCreator : WorldObjectCreator
{
    public enum ObjectType { Sprite, Mesh };
    public WallTemplate wallTemplate;
    public ObjectType objectType = ObjectType.Mesh;
    [Range(1, 50)]
    public int length;
    public int doorPosition;
    public DoorStatus doorStatus = DoorStatus.Wall;
    public int openSize = 3;
    public bool hasCornerLeft;
    public bool hasCornerRight;
    public DoorType doorType = DoorType.Normal;

    private List<GameObject> toInstantiate;

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
    private BoxCollider2D colliderLeft;
    private BoxCollider2D colliderRight;

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
    }

    private void createColliders()
    {
        switch (doorStatus)
        {
            case DoorStatus.Wall:
                if (colliderLeft == null)
                    colliderLeft = gameObject.AddComponent<BoxCollider2D>();

                if (colliderRight != null)
                    DestroyImmediate(colliderRight);

                colliderLeft.size = new Vector2(length, 2);
                if (hasCornerLeft)
                    colliderLeft.size += new Vector2(2, 0);
                if (hasCornerRight)
                    colliderLeft.size += new Vector2(2, 0);

                colliderLeft.offset = new Vector2(length/2f - 0.5f, 0.5f);
                if (hasCornerLeft)
                    colliderLeft.offset -= new Vector2(1, 0);
                if (hasCornerRight)
                    colliderLeft.offset += new Vector2(1, 0);

                break;
            case DoorStatus.Open:
            case DoorStatus.Door:
                if (colliderLeft == null && (doorPosition > 0 || hasCornerLeft))
                    colliderLeft = gameObject.AddComponent<BoxCollider2D>();
                if (colliderRight == null && ((length - (doorPosition + 3) > 0) || hasCornerRight))
                    colliderRight = gameObject.AddComponent<BoxCollider2D>();

                if (colliderLeft != null && doorPosition <= 0 && !hasCornerLeft)
                    DestroyImmediate(colliderLeft);
                if (colliderRight != null && (length - (doorPosition + 3) <= 0) && !hasCornerRight)
                    DestroyImmediate(colliderRight);

                if (colliderLeft != null)
                {
                    colliderLeft.size = new Vector2(doorPosition, 2);
                    colliderLeft.offset = new Vector2(doorPosition / 2f - 0.5f, 0.5f);
                    if (hasCornerLeft)
                    {
                        colliderLeft.size += new Vector2(2, 0);
                        colliderLeft.offset -= new Vector2(1, 0);
                    }
                }

                if (colliderRight != null)
                {
                    colliderRight.size = new Vector2(length - (doorPosition + 3), 2);
                    colliderRight.offset = new Vector2(doorPosition + 3 + (colliderRight.size.x / 2) - 0.5f, 0.5f);
                    if (hasCornerRight)
                    {
                        colliderRight.size += new Vector2(2, 0);
                        colliderRight.offset += new Vector2(1, 0);
                    }
                }
                break;
        }
    }

    public override void refreshContents()
    {
        if (!parentRoom)
            parentRoom = GetComponentInParent<Room>();
        if (!hasChanged())
            return;
        destroyContents();

        switch (objectType)
        {
            case ObjectType.Mesh:
                createWallAsMeshes();
                break;
            case ObjectType.Sprite:
                createWallAsSprites();
                break;
        }

        createColliders();

        currentLength = length;
        currentDoorPosition = doorPosition;
        currentDoorStatus = doorStatus;
        currentHasCornerLeft = hasCornerLeft;
        currentHasCornerRight = hasCornerRight;
        currentOpenSize = openSize;
    }

    private void createWallAsSprites()
    {
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
                        MeshCreator newMesh = Instantiate(wallTemplate.meshCreator);
                        newMesh.size.x = openSize;
                        newMesh.size.y = 2;
                        newMesh.transform.SetParent(transform);
                        newMesh.meshType = MeshCreator.MeshType.Floor;
                        newMesh.tag = "Floor";
                        newMesh.transform.localPosition = new Vector3(i - 0.5f, -0.5f, 0);
                        i += (openSize - 1);
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

        transform.rotation = savedLocalRotation;
    }

    private void createWallAsMeshes()
    {
        Quaternion savedLocalRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        doorPosition = Mathf.Clamp(doorPosition, 0, length - 3);
        openSize = Mathf.Clamp(openSize, 3, length - 4);
        door = null;

        switch (doorStatus)
        {
            case DoorStatus.Wall:
                MeshCreator wall = Instantiate(wallTemplate.meshCreator);
                wall.hasExteriorCornerLeft = hasCornerLeft;
                wall.hasExteriorCornerRight = hasCornerRight;
                wall.transform.SetParent(transform);
                if (wall.hasExteriorCornerLeft)
                    wall.transform.localPosition = new Vector3(-2, 0, 0);
                else
                    wall.transform.localPosition = new Vector3(0, 0, 0);
                wall.transform.localPosition += new Vector3(-0.5f, -0.5f, 0);

                wall.size.y = 2;
                wall.size.x = length;

                if (wall.hasExteriorCornerRight)
                    wall.size.x += 2;
                if (wall.hasExteriorCornerLeft)
                    wall.size.x += 2;

                wall.meshType = MeshCreator.MeshType.Wall;
                wall.BuildMesh();

                break;

            case DoorStatus.Open:
                // The left wall
                MeshCreator leftWall = Instantiate(wallTemplate.meshCreator);
                leftWall.hasExteriorCornerLeft = hasCornerLeft;
                leftWall.transform.SetParent(transform);

                leftWall.size.y = 2;
                leftWall.size.x = doorPosition;
                
                leftWall.transform.localPosition = new Vector3(0, 0, 0);
                if (leftWall.hasExteriorCornerLeft)
                    leftWall.transform.localPosition += new Vector3(-2, 0, 0);

                leftWall.transform.localPosition += new Vector3(-0.5f, -0.5f, 0);

                if (leftWall.hasExteriorCornerLeft)
                    leftWall.size.x += 2;

                leftWall.hasInteriorCornerRight = true;
                leftWall.meshType = MeshCreator.MeshType.Wall;
                leftWall.BuildMesh();

                // The open area
                MeshCreator newMesh = Instantiate(wallTemplate.meshCreator);
                newMesh.size.x = openSize;
                newMesh.size.y = 2;
                newMesh.transform.SetParent(transform);
                newMesh.meshType = MeshCreator.MeshType.Floor;
                newMesh.transform.localPosition = new Vector3(doorPosition - 0.5f, -0.5f, 0);
                newMesh.BuildMesh();

                // The right wall
                MeshCreator rightWall = Instantiate(wallTemplate.meshCreator);
                rightWall.hasExteriorCornerRight = hasCornerRight;
                rightWall.transform.SetParent(transform);

                rightWall.size.y = 2;
                rightWall.size.x = length - (doorPosition+3);
                rightWall.transform.localPosition = new Vector3(doorPosition+3, 0, 0);

                rightWall.transform.localPosition += new Vector3(-0.5f, -0.5f, 0);

                if (rightWall.hasExteriorCornerRight)
                    rightWall.size.x += 2;

                rightWall.hasInteriorCornerLeft = true;
                rightWall.meshType = MeshCreator.MeshType.Wall;
                rightWall.BuildMesh();
                break;

            case DoorStatus.Door:
                // The left wall
                MeshCreator doorLeftWall = Instantiate(wallTemplate.meshCreator);
                doorLeftWall.hasExteriorCornerLeft = hasCornerLeft;
                doorLeftWall.transform.SetParent(transform);

                doorLeftWall.size.y = 2;
                doorLeftWall.size.x = doorPosition;

                doorLeftWall.transform.localPosition = new Vector3(0, 0, 0);
                if (doorLeftWall.hasExteriorCornerLeft)
                    doorLeftWall.transform.localPosition += new Vector3(-2, 0, 0);

                doorLeftWall.transform.localPosition += new Vector3(-0.5f, -0.5f, 0);

                if (doorLeftWall.hasExteriorCornerLeft)
                    doorLeftWall.size.x += 2;

                doorLeftWall.meshType = MeshCreator.MeshType.Wall;
                doorLeftWall.BuildMesh();

                // The door
                door = instantiateContent(new Vector3(doorPosition+1, 0, 0), wallTemplate.getDoor(doorType)).GetComponent<Door>();

                // The right wall
                MeshCreator doorRightWall = Instantiate(wallTemplate.meshCreator);
                doorRightWall.hasExteriorCornerRight = hasCornerRight;
                doorRightWall.transform.SetParent(transform);

                doorRightWall.size.y = 2;
                doorRightWall.size.x = length - (doorPosition + 3);
                doorRightWall.transform.localPosition = new Vector3(doorPosition + 3, 0, 0);

                doorRightWall.transform.localPosition += new Vector3(-0.5f, -0.5f, 0);

                if (doorRightWall.hasExteriorCornerRight)
                    doorRightWall.size.x += 2;

                doorRightWall.meshType = MeshCreator.MeshType.Wall;
                doorRightWall.BuildMesh();
                break;
        }

        transform.rotation = savedLocalRotation;
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
