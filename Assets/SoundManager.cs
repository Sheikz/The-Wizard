using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager instance;
    [Tooltip("Interval at which the same audio clip can be played")]
    public float soundCooldown = 0.25f;
    public GameObject musicManager;
    public AudioClip dungeonMusic;

    public AudioClip bossMusic;
    public AudioClip[] sounds;

    private AudioSource musicSource;
    private AudioSource[] audioSources;
    private GameObject[] audioEmitters;
    private List<string> playingSounds;
    private int currentAudioSource = 0;

    private float volumeSFX = 1;
    private float volumeMusic = 1;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        loadSounds();
        audioSources = GetComponents<AudioSource>();
        musicSource = musicManager.GetComponent<AudioSource>();
        audioEmitters = new GameObject[audioSources.Length];
        playingSounds = new List<string>();
    }

    void Start()
    {
        if (!Application.isEditor)
            playDungeonMusic();
    }

    public float SFXVolume
    {
        get { return volumeSFX;  }
        set {
            volumeSFX = value;
            refreshVolumeSFX();
        }
    }

    public float MusicVolume
    {
        get { return volumeMusic; }
        set
        {
            volumeMusic = value;
            refreshVolumeMusic();
        }
    }

    public void playDungeonMusic()
    {
        if (musicSource.clip == dungeonMusic && musicSource.isPlaying)
            return;

        musicSource.clip = dungeonMusic;
        musicSource.Play();
    }

    public void playBossMusic()
    {
        if (musicSource.clip == bossMusic && musicSource.isPlaying)
            return;

        musicSource.clip = bossMusic;
        musicSource.Play();
    }

    /// <summary>
    /// Load all the sounds at runtime from the Resources folder
    /// </summary>
    public void loadSounds()
    {
        sounds = Resources.LoadAll<AudioClip>("Sounds/");
        foreach (AudioClip clip in sounds)
        {
            clip.name = clip.name.Replace("_", "");
        }
    }

    public void playSound(string clipName, GameObject emitter = null)
    {
        if (clipName == "")
        {
            //Debug.LogError("empty audioclip!");
            return;
        }

        clipName = clipName.Replace(" ", "");
        if (playingSounds.Contains(clipName))
            return;
        List<AudioClip> clips = new List<AudioClip>();
        foreach (AudioClip clip in sounds)
        {
            if (!clip)
                continue;

            if (clip.name == clipName)
            {
                clips.Add(clip);
                continue;
            }
            else if (clip.name.EndsWith("Hit"))
                continue;
            else if (clip.name.Contains(clipName))
            {
                clips.Add(clip);
                continue;
            }
        }
        if (clips.Count == 0)
        {
            //Debug.LogWarning("No audioclip found for " + clipName);
            return;
        }

        //audio.clip = Utils.pickRandom(clips);
        play(Utils.pickRandom(clips), emitter);
        StartCoroutine(routinePlayingSound(clipName));
    }

    private void refreshVolumeSFX()
    {
        foreach (AudioSource source in audioSources)
        {
            source.volume = volumeSFX;
        }
    }

    private void refreshVolumeMusic()
    {
        musicSource.volume = volumeMusic;
    }

    public void stopSoundFromMe(GameObject emitter)
    {
        stopClipFromEmitter(emitter);
    }

    private void play(AudioClip audioClip, GameObject emitter = null)
    {
        audioSources[currentAudioSource].clip = audioClip;
        audioSources[currentAudioSource].Play();
        if (emitter != null)
            audioEmitters[currentAudioSource] = emitter;
        currentAudioSource++;
        currentAudioSource %= audioSources.Length;
    }

    private void stopClipFromEmitter(GameObject emitter)
    {
        if (GameManager.instance.isShuttingDown)
            return;

        for (int i=0 ; i < audioEmitters.Length; i++)
        {
            if (audioEmitters[i] == emitter)
                audioSources[i].Stop();
        }
    }

    /// <summary>
    /// Put a cooldown between each sound to avoid clustering
    /// </summary>
    /// <param name="clipName"></param>
    /// <returns></returns>
    IEnumerator routinePlayingSound(string clipName)
    {
        if (clipName.StartsWith("Click"))
            yield break; // Not cooldown for UI sounds

        playingSounds.Add(clipName);
        yield return new WaitForSecondsRealtime(soundCooldown);
        playingSounds.Remove(clipName);
    }
}
