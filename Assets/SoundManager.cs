using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager instance;
    [Tooltip("Interval at which the same audio clip can be played")]
    public float soundCooldown = 0.25f;
    public AudioClip[] sounds;

    private AudioSource[] audioSources;
    private GameObject[] audioEmitters;
    private List<string> playingSounds;
    private int currentAudioSource = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        loadSounds();
        audioSources = GetComponents<AudioSource>();
        audioEmitters = new GameObject[audioSources.Length];
        playingSounds = new List<string>();
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
