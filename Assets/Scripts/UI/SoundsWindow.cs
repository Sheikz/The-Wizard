using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundsWindow : MonoBehaviour {

    public Slider SFXSlider;
    public Slider MusicSlider;

    void Start()
    {
        SFXSlider.value = SoundManager.instance.SFXVolume;
        MusicSlider.value = SoundManager.instance.MusicVolume;
    }
}
