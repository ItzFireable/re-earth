using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    // Audio source
    [SerializeField] private AudioSource source;

    // Audio clips
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip fallSound;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip heavyAttackSound;

    // Used for playing sound
    public void PlaySound(string type, int? num)
    {
        // Set loopable to false and switch types, and get the correct audio for each type
        source.loop = false;
        switch(type)
        {
            case "Attack":
                // Get sound for attack from the list 
                AudioClip sound = attackSounds[((int) num) - 1];
                source.clip = sound;
                source.Play(0);
                break;
            case "HeavyAttack":
                source.clip = heavyAttackSound;
                source.Play(0);
                break;
            case "Jump":
                source.clip = jumpSound;
                source.Play(0);
                break;
            case "Run":
                source.clip = runSound;
                source.Play(0);
                break;
            case "Fall":
                source.loop = true;
                source.clip = fallSound;
                source.Play(0);
                break;
        }
    }
}
