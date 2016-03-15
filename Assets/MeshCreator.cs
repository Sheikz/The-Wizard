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
    public enum MeshType { Floor, Hole, Deco1, Deco2, Carpet};
    public MeshType meshType = MeshType.Floor;
    public Vector2i size = new Vector2i(5, 5);
    public float tileSize = 1.0f;
    public Texture2D texturePrefab;
    public int tileResolution;
    public Material prefabMaterial;
    public bool hasCollider = false;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Vector2i[] floorTiles;
    private Color[][] pixels;
    new private BoxCollider2D collider;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        BuildMesh();
    }

    void PrepareTextures()
    {
        switch (meshType)
        {
            case MeshType.Floor:
                pixels = new Color[2][];
                pixels[0] = texturePrefab.GetPixels(0 * tileResolution, 15 * tileResolution, tileResolution, tileResolution);
                pixels[1] = texturePrefab.GetPixels(1 * tileResolution, 15 * tileResolution, tileResolution, tileResolution);
                tag = "Floor";
                break;
            case MeshType.Hole:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = texturePrefab.GetPixels(14 * tileResolution, 5 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthWest] = texturePrefab.GetPixels(13 * tileResolution, 6 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.North] = texturePrefab.GetPixels(14 * tileResolution, 6 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthEast] = texturePrefab.GetPixels(15 * tileResolution, 6 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.East] = texturePrefab.GetPixels(15 * tileResolution, 5 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthEast] = texturePrefab.GetPixels(15 * tileResolution, 4 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.South] = texturePrefab.GetPixels(14 * tileResolution, 4 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthWest] = texturePrefab.GetPixels(13 * tileResolution, 4 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.West] = texturePrefab.GetPixels(13 * tileResolution, 5 * tileResolution, tileResolution, tileResolution);
                tag = "Hole";
                break;
            case MeshType.Carpet:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = texturePrefab.GetPixels(14 * tileResolution, 8 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthWest] = texturePrefab.GetPixels(13 * tileResolution, 9 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.North] = texturePrefab.GetPixels(14 * tileResolution, 9 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthEast] = texturePrefab.GetPixels(15 * tileResolution, 9 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.East] = texturePrefab.GetPixels(15 * tileResolution, 8 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthEast] = texturePrefab.GetPixels(15 * tileResolution, 7 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.South] = texturePrefab.GetPixels(14 * tileResolution, 7 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthWest] = texturePrefab.GetPixels(13 * tileResolution, 7 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.West] = texturePrefab.GetPixels(13 * tileResolution, 8 * tileResolution, tileResolution, tileResolution);
                tag = "Untagged";
                break;
            case MeshType.Deco1:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = texturePrefab.GetPixels(12 * tileResolution, 10 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthWest] = texturePrefab.GetPixels(13 * tileResolution, 12 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.North] = texturePrefab.GetPixels(14 * tileResolution, 12 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthEast] = texturePrefab.GetPixels(15 * tileResolution, 12 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.East] = texturePrefab.GetPixels(15 * tileResolution, 11 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthEast] = texturePrefab.GetPixels(15 * tileResolution, 10 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.South] = texturePrefab.GetPixels(14 * tileResolution, 10 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthWest] = texturePrefab.GetPixels(13 * tileResolution, 10 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.West] = texturePrefab.GetPixels(13 * tileResolution, 11 * tileResolution, tileResolution, tileResolution);
                tag = "Untagged";
                break;
            case MeshType.Deco2:
                pixels = new Color[9][];
                pixels[(int)TextureItem.Center] = texturePrefab.GetPixels(12 * tileResolution, 10 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthWest] = texturePrefab.GetPixels(13 * tileResolution, 15 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.North] = texturePrefab.GetPixels(14 * tileResolution, 15 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.NorthEast] = texturePrefab.GetPixels(15 * tileResolution, 15 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.East] = texturePrefab.GetPixels(15 * tileResolution, 14 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthEast] = texturePrefab.GetPixels(15 * tileResolution, 13 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.South] = texturePrefab.GetPixels(14 * tileResolution, 13 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.SouthWest] = texturePrefab.GetPixels(13 * tileResolution, 13 * tileResolution, tileResolution, tileResolution);
                pixels[(int)TextureItem.West] = texturePrefab.GetPixels(13 * tileResolution, 14 * tileResolution, tileResolution, tileResolution);
                tag = "Untagged";
                break;
        }
        
    }

    void BuildTexture()
    {
        PrepareTextures();
        Texture2D texture = new Texture2D(size.x* tileResolution, size.y*tileResolution);
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (meshType == MeshType.Floor)
                    texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, pixels[Random.Range(0, pixels.Length)]);
                else
                    texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, getTextureItem(x, y));
            }
        }

        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        Material mat = new Material(prefabMaterial);
        mat.mainTexture = texture;
        meshRenderer.material = mat;
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

        meshFilter.mesh = mesh;

        BuildTexture();
        AlignToAxis();
        CreateCollider();
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
        collider.isTrigger = true;
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

    public void AlignToAxis()
    {
        Vector3 p = transform.localPosition;
        p.x = Mathf.RoundToInt(p.x*2)/2f;
        p.y = Mathf.RoundToInt(p.y*2)/2f;
        p.z = Mathf.RoundToInt(p.z*2)/2f;
        transform.localPosition = p;
    }

}
