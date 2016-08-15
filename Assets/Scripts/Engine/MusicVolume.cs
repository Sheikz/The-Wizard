using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour 
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

    public void setMusicVolume(float value)
    {
        SoundManager.instance.MusicVolume = value;
        refreshText();
    }

    void refreshText()
    {
        sfxText.text = "Music: " + Mathf.RoundToInt(SoundManager.instance.MusicVolume * 100);
    }
}
