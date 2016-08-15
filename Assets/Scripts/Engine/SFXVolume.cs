using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SFXVolume : MonoBehaviour 
{
    private Text sfxText;

    void Awake()
    {
        sfxText = GetComponent<Text>();
    }

    void Start()
    {
        refreshText();
    }

    public void setSFXVolume(float value)
    {
        SoundManager.instance.SFXVolume = value;
        refreshText();
    }

    void refreshText()
    {
        sfxText.text = "SFX: " + Mathf.RoundToInt(SoundManager.instance.SFXVolume * 100);
    }
}
