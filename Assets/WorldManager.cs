using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public Pillar pillar;
    public Pillar pillarWithTorch;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        name = "WorldManager";
    }
}
