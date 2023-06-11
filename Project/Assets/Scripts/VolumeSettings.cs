using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetVolume(string type, float volume)
    {
        audioMixer.SetFloat(type, volume);
    }
}
