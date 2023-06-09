using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    // Audio source
    [SerializeField] private AudioSource source;

    // Audio clips
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip damageSound;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip heavyAttackSound;

    string curSound = "";

    // Used for playing sound
    public void PlaySound(string type, int? num = 0, bool loop = false)
    {
        if (curSound == type && loop) return;
        curSound = type;

        // Set loopable to false and switch types, and get the correct audio for each type
        source.loop = false;
        source.volume = 1f;
        
        switch(type)
        {
            case "Attack":
                AudioClip sound = attackSounds[((int) num) - 1];
                source.clip = sound;
                break;
            case "HeavyAttack":
                source.clip = heavyAttackSound;
                break;
            case "Dash":
                source.clip = dashSound;
                source.volume = 0.5f;
                break;
            case "Run":
                source.loop = true;
                source.clip = runSound;
                source.volume = 0.75f;
                break;
            case "Damage":
                source.clip = damageSound;
                source.volume = 0.5f;
                break;
        }
        source.Play(0);

    }

    public void StopSound(string type)
    {
        if (curSound == type)
        {
            source.Stop();
            curSound = "";
        }
    }
}
