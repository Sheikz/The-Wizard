using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCreator : MonoBehaviour 
{
    private enum TextureItem { Center, NorthWest, North, NorthEast, East, SouthEast, South, SouthWest, West };
    public enum MeshType { Floor, Hole, Deco1, Deco2, Carpet, Wall, WallCorner, Roof};
    public enum DecoType { Exterior, Interior };
    public MeshType meshType = MeshType.Floor;
    public Vector2i size = new Vector2i(5, 5);
    public float tileSize = 1.0f;
    public int tileResolution;
    public Material prefabMaterial;
    public bool hasCollider = false;
    [HideInInspector]
    public bool hasInteriorCornerLeft = false;
    [HideInInspector]
    public bool hasInteriorCornerRight = false;
    [HideInInspector]
    public bool hasExteriorCornerLeft = false;
    [HideInInspector]
    public bool hasExteriorCornerRight = false;
    [HideInInspector]
    public bool blockingLow = false;
    [HideInInspector]
    public DecoType decoType = DecoType.Exterior;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Vector2i[] floorTiles;
    private Texture2D texturePrefab;
    private Color[][] pixels;
    private Color[] interiorCornerLeft;
    private Color[] interiorCornerRight;
    private Color[] exteriorCornerLeft;
    private Color[] exteriorCornerRight;
    private List<int> noDecoPositions;
    new private BoxCollider2D collider;

    struct IndexWithDropChance
    {
        public int index;
        public float chance;

        public static int getRandom(List<IndexWithDropChance> input)
        {
            if (input == null)
                return 0;

            float totalSum = 0f;
            foreach (IndexWithDropChance obj in input)
            {
                totalSum += obj.chance;
            }
            float index = Random.Range(0, totalSum);
            float sum = 0f;
            int i = 0;
            while (sum < index)
            {
                sum += input[i++].chance;
            }
            return input[Math.Max(0, i - 1)].index;
        }
    };

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        noDecoPositions = new List<int>();
    }

    void PrepareTextures()
    {
        texturePrefab = WorldManager.instance.getDungeonTileSet();
        switch (meshType)
        {
            case MeshType.Floor:
                pixels = new Color[2][];
                pixels[0] = getPixels(0, 15);
                pixels[1] = getPixels(1, 15);
                tag = "Floor";
                name = "Floor";
                meshRenderer.sortingOrder = 0;
                meshRenderer.sortingLayerName = "Floor";
                break;
            case MeshType.Hole:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = getPixels(14, 5);
                pixels[(int)TextureItem.NorthWest] = getPixels(13, 6);
                pixels[(int)TextureItem.North] = getPixels(14, 6);
                pixels[(int)TextureItem.NorthEast] = getPixels(15, 6);
                pixels[(int)TextureItem.East] = getPixels(15, 5);
                pixels[(int)TextureItem.SouthEast] = getPixels(15, 4);
                pixels[(int)TextureItem.South] = getPixels(14, 4);
                pixels[(int)TextureItem.SouthWest] = getPixels(13, 4);
                pixels[(int)TextureItem.West] = getPixels(13, 5);
                tag = "Hole";
                name = "Hole";
                gameObject.layer = LayerManager.instance.obstaclesLayerInt;
                meshRenderer.sortingOrder = 3;
                meshRenderer.sortingLayerName = "Floor";
                break;
            case MeshType.Carpet:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = getPixels(14, 8);
                pixels[(int)TextureItem.NorthWest] = getPixels(13, 9);
                pixels[(int)TextureItem.North] = getPixels(14, 9);
                pixels[(int)TextureItem.NorthEast] = getPixels(15, 9);
                pixels[(int)TextureItem.East] = getPixels(15, 8);
                pixels[(int)TextureItem.SouthEast] = getPixels(15, 7);
                pixels[(int)TextureItem.South] = getPixels(14, 7);
                pixels[(int)TextureItem.SouthWest] = getPixels(13, 7);
                pixels[(int)TextureItem.West] = getPixels(13, 8);
                tag = "Untagged";
                name = "Carpet";
                meshRenderer.sortingOrder = 2;
                meshRenderer.sortingLayerName = "Floor";
                break;
            case MeshType.Deco1:
                pixels = new Color[9][];
                if (decoType == DecoType.Interior)
                {
                    pixels[(int)TextureItem.Center] = getPixels(12, 10);
                    pixels[(int)TextureItem.NorthWest] = getPixels(11, 12);
                    pixels[(int)TextureItem.North] = getPixels(14, 10);
                    pixels[(int)TextureItem.NorthEast] = getPixels(12, 12);
                    pixels[(int)TextureItem.East] = getPixels(13, 11);
                    pixels[(int)TextureItem.SouthEast] = getPixels(12, 11);
                    pixels[(int)TextureItem.South] = getPixels(14, 12);
                    pixels[(int)TextureItem.SouthWest] = getPixels(11, 11);
                    pixels[(int)TextureItem.West] = getPixels(15, 11);
                }
                else
                {
                    pixels[(int)TextureItem.Center] = getPixels(12, 10);
                    pixels[(int)TextureItem.NorthWest] = getPixels(13, 12);
                    pixels[(int)TextureItem.North] = getPixels(14, 12);
                    pixels[(int)TextureItem.NorthEast] = getPixels(15, 12);
                    pixels[(int)TextureItem.East] = getPixels(15, 11);
                    pixels[(int)TextureItem.SouthEast] = getPixels(15, 10);
                    pixels[(int)TextureItem.South] = getPixels(14, 10);
                    pixels[(int)TextureItem.SouthWest] = getPixels(13, 10);
                    pixels[(int)TextureItem.West] = getPixels(13, 11);
                }
                tag = "Untagged";
                name = "FloorDeco";
                meshRenderer.sortingOrder = 1;
                meshRenderer.sortingLayerName = "Floor";
                break;
            case MeshType.Deco2:
                pixels = new Color[9][];
                if (decoType == DecoType.Interior)
                {
                    pixels[(int)TextureItem.Center] = getPixels(12, 10);
                    pixels[(int)TextureItem.NorthWest] = getPixels(11, 14);
                    pixels[(int)TextureItem.North] = getPixels(14, 13);
                    pixels[(int)TextureItem.NorthEast] = getPixels(12, 14);
                    pixels[(int)TextureItem.East] = getPixels(13, 14);
                    pixels[(int)TextureItem.SouthEast] = getPixels(12, 13);
                    pixels[(int)TextureItem.South] = getPixels(14, 15);
                    pixels[(int)TextureItem.SouthWest] = getPixels(11, 13);
                    pixels[(int)TextureItem.West] = getPixels(15, 14);
                }
                else
                {
                    pixels[(int)TextureItem.Center] = getPixels(12, 10);
                    pixels[(int)TextureItem.NorthWest] = getPixels(13, 15);
                    pixels[(int)TextureItem.North] = getPixels(14, 15);
                    pixels[(int)TextureItem.NorthEast] = getPixels(15, 15);
                    pixels[(int)TextureItem.East] = getPixels(15, 14);
                    pixels[(int)TextureItem.SouthEast] = getPixels(15, 13);
                    pixels[(int)TextureItem.South] = getPixels(14, 13);
                    pixels[(int)TextureItem.SouthWest] = getPixels(13, 13);
                    pixels[(int)TextureItem.West] = getPixels(13, 14);
                }
                tag = "Untagged";
                name = "FloorDeco";
                meshRenderer.sortingOrder = 1;
                meshRenderer.sortingLayerName = "Floor";
                break;
            case MeshType.Wall:
                pixels = new Color[6][];
                pixels[0] = getPixels(2, 13, 1, 2);
                pixels[1] = getPixels(3, 13, 1, 2);
                pixels[2] = getPixels(4, 13, 1, 2);
                pixels[3] = getPixels(9, 9, 1, 2);
                pixels[4] = getPixels(10, 9, 1, 2);
                pixels[5] = getPixels(5, 3, 1, 2);

                interiorCornerLeft = getPixels(7, 11, 2, 2);
                interiorCornerRight = getPixels(9, 11, 2, 2);

                exteriorCornerLeft = getPixels(0, 13, 2, 2);
                exteriorCornerRight = getPixels(5, 13, 2, 2);
                tag = "Wall";
                name = "Wall";
                if (blockingLow)
                    gameObject.layer = LayerManager.instance.blockingLowInt;
                else
                    gameObject.layer = LayerManager.instance.blockingLayerInt;
                meshRenderer.sortingOrder = 0;
                meshRenderer.sortingLayerName = "Walls";
                break;
            case MeshType.WallCorner:
                pixels = new Color[1][];
                pixels[0] = getPixels(7, 11, 2, 2);
                tag = "Wall";
                name = "Corner";
                meshRenderer.sortingOrder = 0;
                meshRenderer.sortingLayerName = "Walls";
                break;
            case MeshType.Roof:
                pixels = new Color[2][];
                pixels[0] = getPixels(4, 15);
                pixels[1] = getPixels(5, 15);
                tag = "Roof";
                name = "Roof";
                meshRenderer.sortingOrder = 0;
                meshRenderer.sortingLayerName = "Walls";
                break;
        }
        
    }

    void BuildTexture()
    {
        PrepareTextures();
        Texture2D texture = new Texture2D(size.x* tileResolution, size.y*tileResolution);
        if (meshType == MeshType.Wall)  // If it's a wall, the texture mapping is quite different
        {
            mapWallTextures(texture);
        }
        else if (meshType == MeshType.WallCorner)
        {
            setPixels(texture, 0, 0, 2, 2, pixels[0]);
        }
        else  // We go through each tile and apply the correct texture from the tileset
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    switch (meshType)
                    {
                        case MeshType.Floor:
                        case MeshType.Roof:
                            setPixels(texture, x, y, 1, 1, pixels[Random.Range(0, pixels.Length)]);
                            break;
                        default:
                            setPixels(texture, x, y, 1, 1, getTextureItem(x, y));
                            break;
                    }
                }
            }
        }

        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        Material mat = new Material(prefabMaterial);
        mat.mainTexture = texture;
        meshRenderer.material = mat;
    }

    private void mapWallTextures(Texture2D texture)
    {
        bool lastIsWindow = false;
        bool lastIsPillar = false;
        List<IndexWithDropChance> wallIndexes = new List<IndexWithDropChance>();
        for (int x = 0; x < size.x; x++)
        {
            if (hasInteriorCornerLeft && size.x >= 2 && x ==0)
            {
                setPixels(texture, x, 0,  2,  2, interiorCornerLeft);
                x++;
                continue;
            }
            else if (hasExteriorCornerLeft && size.x >= 2 && x == 0)
            {
                setPixels(texture, x, 0, 2, 2, exteriorCornerLeft);
                x++;
                continue;
            }
            if (hasInteriorCornerRight && size.x >= 2 && x == size.x-2)
            {
                setPixels(texture, x, 0, 2, 2, interiorCornerRight);
                x++;
                continue;
            }
            else if (hasExteriorCornerRight && size.x >= 2 && x == size.x - 2)
            {
                setPixels(texture, x, 0, 2, 2, exteriorCornerRight);
                x++;
                continue;
            }

            wallIndexes.Clear();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (i == 5 && lastIsWindow)
                    continue;

                if (i == 1 && lastIsPillar)
                    continue;

                float chance = 1f;
                if (i == 5)
                    chance = 0.5f;  // Window is less likely
                else if (i == 3)
                    chance = 0.5f;  // Rumbles is less likely
                else if (i == 1)
                    chance = 0.5f;  // Pillar is less likely

                IndexWithDropChance index;
                index.index = i;
                index.chance = chance;
                wallIndexes.Add(index);
            }

            int wallType = IndexWithDropChance.getRandom(wallIndexes);

            lastIsWindow = false;
            lastIsPillar = false;
            setPixels(texture, x , 0, 1, 2, pixels[wallType]);
            if (wallType == 5) // Wall with window
            {
                noDecoPositions.Add(x); // Avoid decoration on windows
                GameObject light = Instantiate(WorldManager.instance.windowLight);
                light.transform.SetParent(transform);
                light.transform.localPosition = new Vector3(x, 0, 0) + new Vector3(0.5f, 0.5f, 0);
                light.transform.rotation = transform.rotation;
                lastIsWindow = true;
            }
            else if (wallType == 3) // Wall with rumbles
            {
                noDecoPositions.Add(x);
                GameObject rumbles = Instantiate(WorldManager.instance.wallRumbles);
                rumbles.transform.SetParent(transform);
                rumbles.transform.localPosition = new Vector3(x, 0, 0) + new Vector3(0.5f, 0.5f, 0);
                rumbles.transform.rotation = transform.rotation;
                rumbles.layer = LayerManager.instance.blockingLowInt;
            }
            else if (wallType == 1) // Pillar in-wall
            {
                lastIsPillar = true;
                noDecoPositions.Add(x);
            }
        }
    }

    private Color[] getTextureItem(int x, int y)
    {
        if (x == 0)
        {
            if (y == 0)             // Bottom left corner
            {
                return pixels[(int)TextureItem.SouthWest];
            }
            else if (y == size.y - 1)   // Top left corner
            {
                return pixels[(int)TextureItem.NorthWest];
            }
            return pixels[(int)TextureItem.West];
        }
        else if (x == size.x - 1)  // Right side
        {
            if (y == 0)         // Bottom right corner
            {
                return pixels[(int)TextureItem.SouthEast];
            }
            else if (y == size.y - 1)   // Top Right corner
            {
                return pixels[(int)TextureItem.NorthEast];
            }
            return pixels[(int)TextureItem.East];
        }
        else if (y == 0)    // Bottom side
        {
            return pixels[(int)TextureItem.South];
        }
        else if (y == size.y - 1) // Top side
        {
            return pixels[(int)TextureItem.North];
        }
        else // Center
            return pixels[(int)TextureItem.Center];
    }

    public void BuildMesh()
    {
        clearContents();

        if (size.x <= 0 || size.y <= 0)
        {
            DestroyImmediate(gameObject);
            return;
        }
        if (meshType == MeshType.Wall)
        {
            hasCollider = true;
            size.y = 2; // Forcing the height to 2 as it is mandatory for our wall tileset
        }
        else if (meshType == MeshType.WallCorner)
        {
            size.x = 2;
            size.y = 2;
            hasCollider = true;
        }

        int numTiles = size.x * size.y;
        int numTris = numTiles * 2;

        Vector2i vSize = new Vector2i(size.x + 1, size.y + 1);
        int numVerts = vSize.x * vSize.y;

        // Generate the mesh data
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];

        int x, y;
        for (y = 0; y < vSize.y; y++)
        {
            for (x = 0; x < vSize.x; x++)
            {
                vertices[y * vSize.x + x] = new Vector3(x * tileSize, y * tileSize, 0);
                normals[y * vSize.x + x] = Vector3.back;
                uv[y * vSize.x + x] = new Vector2((float)x / size.x, (float)y/size.y);
            }
        }

        for (y = 0; y < size.y; y++)
        {
            for (x = 0; x < size.x; x++)
            {
                int squareIndex = y * size.x + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = y * vSize.x + x + 0;
                triangles[triOffset + 1] = y * vSize.x + x + vSize.x + 0;
                triangles[triOffset + 2] = y * vSize.x + x + vSize.x + 1;

                triangles[triOffset + 3] = y * vSize.x + x + 0;
                triangles[triOffset + 4] = y * vSize.x + x + vSize.x + 1;
                triangles[triOffset + 5] = y * vSize.x + x + 1;
            }
        }


        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;

        if (!meshFilter)
            return;

        meshFilter.mesh = mesh;

        BuildTexture();
        AlignToAxis();
        CreateCollider();
    }

    private void clearContents()
    {
        noDecoPositions.Clear();
        List<Transform> toDestroy = new List<Transform>();
        List<Collider2D> toDestroy2 = new List<Collider2D>();

        foreach (Transform child in transform)
        {
            toDestroy.Add(child);
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            toDestroy2.Add(col);
        }

        foreach (Transform t in toDestroy)
        {
            DestroyImmediate(t.gameObject);
        }

        foreach(Collider2D col in toDestroy2)
        {
            DestroyImmediate(col);
        }
    }

    void CreateCollider()
    {
        if (!hasCollider)
        {
            if (collider != null)
                DestroyImmediate(collider);
            return;
        }
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(size.x, size.y);
        collider.offset = new Vector2(size.x / 2f, size.y / 2f);
        if (meshType == MeshType.Hole)
            collider.isTrigger = true;
        else
            collider.isTrigger = false;
    }

    /// <summary>
    /// Return the world positions of each tile in the mesh
    /// </summary>
    /// <returns>The positions.</returns>
    public List<Vector3> getPositions()
    {
        Vector3 offset = new Vector3(0.5f, 0.5f);
        List<Vector3> result = new List<Vector3>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                result.Add(transform.rotation *(new Vector3(x, y) + offset) + transform.position);
            }
        }
        return result;
    }

    public List<Vector3> getDecoPositions()
    {
        if (meshType != MeshType.Wall)
            return null;

        int start = 0;
        int end = size.x;
        if (hasExteriorCornerLeft || hasInteriorCornerLeft)
            start += 2;
        if (hasExteriorCornerRight || hasInteriorCornerRight)
            end -= 2;

        Vector3 offset = new Vector3(0.5f, 0.5f);
        List<Vector3> result = new List<Vector3>();
        for (int x = start; x < end; x++)
        {
            if (noDecoPositions.Contains(x))
                continue;
            result.Add(transform.rotation * (new Vector3(x, 0) + offset) + transform.position);
        }
        return result;
    }

    public void AlignToAxis()
    {
        Vector3 p = transform.localPosition;
        p.x = Mathf.RoundToInt(p.x*2)/2f;
        p.y = Mathf.RoundToInt(p.y*2)/2f;
        p.z = Mathf.RoundToInt(p.z*2)/2f;
        transform.localPosition = p;
    }

    /// <summary>
    /// Get the pixels from the spriteSheet at a specific coord
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    Color[] getPixels(int x, int y, int width = 1, int height = 1)
    {
        return texturePrefab.GetPixels(x * tileResolution, y * tileResolution, tileResolution*width, tileResolution*height);
    }

    void setPixels(Texture2D texture, int x, int y, int width, int height, Color[] pixels)
    {
        texture.SetPixels(x * tileResolution, y * tileResolution, width * tileResolution, height * tileResolution, pixels);
    }
}
