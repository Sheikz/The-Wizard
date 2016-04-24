using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdjustTheme : MonoBehaviour 
{
    public enum ThemeObject { SkullDeco, SwordDeco, ShieldDeco,
        BannerTop, BannerBottom, Rumbles,
        DoorLeft, DoorRight, Floor,
        BigDoorLeft, BigDoorRight,
        DoorTopLeft, DoorTopRight,
        BigDoorTopLeft, BigDoorTopRight,
        StairTopLeft, StairsTopRight,
        StairsBottomLeft, StairsBottomRight
    }

    public ThemeObject themeObject;

    void Start()
    {
        SpriteRenderer rdr = GetComponent<SpriteRenderer>();
        if (!rdr)
            return;

        switch (themeObject)
        {
            case ThemeObject.SkullDeco:
                rdr.sprite = WorldManager.instance.skullDeco[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.ShieldDeco:
                rdr.sprite = WorldManager.instance.shieldDeco[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.SwordDeco:
                rdr.sprite = WorldManager.instance.swordDeco[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BannerTop:
                rdr.sprite = WorldManager.instance.bannerDecoTop[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BannerBottom:
                rdr.sprite = WorldManager.instance.bannerDecoBottom[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.Rumbles:
                rdr.sprite = WorldManager.instance.rumbleSprites[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.DoorLeft:
                rdr.sprite = WorldManager.instance.doorLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.DoorRight:
                rdr.sprite = WorldManager.instance.doorRight[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.Floor:
                rdr.sprite = WorldManager.instance.floor[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BigDoorLeft:
                rdr.sprite = WorldManager.instance.bigDoorLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BigDoorRight:
                rdr.sprite = WorldManager.instance.bigDoorRight[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.DoorTopLeft:
                rdr.sprite = WorldManager.instance.doorTopLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.DoorTopRight:
                rdr.sprite = WorldManager.instance.doorTopRight[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BigDoorTopLeft:
                rdr.sprite = WorldManager.instance.bigDoorTopLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.BigDoorTopRight:
                rdr.sprite = WorldManager.instance.bigDoorTopRight[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.StairsTopRight:
                rdr.sprite = WorldManager.instance.stairsTopRight[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.StairTopLeft:
                rdr.sprite = WorldManager.instance.stairsTopLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.StairsBottomLeft:
                rdr.sprite = WorldManager.instance.stairsBottomLeft[WorldManager.instance.getDungeonTheme()];
                break;
            case ThemeObject.StairsBottomRight:
                rdr.sprite = WorldManager.instance.stairsBottomRight[WorldManager.instance.getDungeonTheme()];
                break;
        }
    }
}
