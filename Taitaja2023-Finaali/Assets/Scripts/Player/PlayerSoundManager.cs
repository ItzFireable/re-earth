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
    public void PlaySound(string type, int? num = 0)
    {
        if (curSound == type) return;
        curSound = type;

        // Set loopable to false and switch types, and get the correct audio for each type
        source.loop = false;
        source.volume = 1f;
        
        switch(type)
        {
            case "Attack":
                AudioClip sound = attackSounds[((int) num) - 1];
                source.clip = sound;
                source.Play(0);
                break;
            case "HeavyAttack":
                source.clip = heavyAttackSound;
                source.Play(0);
                break;
            case "Dash":
                source.clip = dashSound;
                source.volume = 0.5f;
                source.Play(0);
                break;
            case "Run":
                source.loop = true;
                source.clip = runSound;
                source.volume = 0.75f;
                source.Play(0);
                break;
            case "Damage":
                source.loop = true;
                source.clip = damageSound;
                source.volume = 0.5f;
                source.Play(0);
                break;
        }
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
