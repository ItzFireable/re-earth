using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    // Audio source
    [SerializeField] private AudioSource source;

    // Audio clips
    [SerializeField] private List<SoundClip> soundClips;

    string curSound = "";

    // Used for playing sound
    public void PlaySound(string type, int? num = 0, bool loop = false)
    {
        if (curSound == type && loop) return;
        curSound = type;

        // Set loopable to false and switch types, and get the correct audio for each type
        source.loop = false;
        source.volume = 1f;
        
        SoundClip clip = soundClips.Find(x => x.type == type);

        if (clip == null) return;

        source.clip = clip.clip;
        source.loop = clip.loop;
        source.volume = clip.volume;

        source.Play();

    }

    // Most sound clips are played as one shots
    public void PlayOneShot(string type, int? num = 0) 
    {
        SoundClip clip = soundClips.Find(x => x.type == type);

        if (clip == null) return;

        clip.Play(source);

    }

    public void StopSound(string type)
    {
        if (curSound == type)
        {
            source.loop = false;
            curSound = "";
        }
    }
}

[System.Serializable]
public class SoundClip
{
    public string type;
    public AudioClip clip;
    public bool loop;

    [Range(0f, 1f)]
    public float volume = 1f;

    public SoundClip(string type, AudioClip clip, bool loop, float volume = 1f)
    {
        this.type = type;
        this.clip = clip;
        this.loop = loop;
        this.volume = volume;
    }

    public void Play(AudioSource source)
    {
        source.PlayOneShot(clip, volume);
    }

}