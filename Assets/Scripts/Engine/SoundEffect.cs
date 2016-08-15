using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffect : MonoBehaviour 
{
    public enum WorldSoundEffect { DestroyVase, DestroyBarrel };
    public WorldSoundEffect type;

	void Start () 
	{
	    switch (type)
        {
            case WorldSoundEffect.DestroyVase:
                SoundManager.instance.playSound("DestroyVase");
                break;
            case WorldSoundEffect.DestroyBarrel:
                SoundManager.instance.playSound("DestroyBarrel");
                break;
        }
	}
}
