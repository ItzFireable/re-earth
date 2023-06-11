using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetVolume(string type, float volume)
    {
        float db = Mathf.Log10(volume) * 20;
        if(volume == 0) db = -80;
        audioMixer.SetFloat(type, db);
    }

    public float GetVolume(string type)
    {
        float db;
        audioMixer.GetFloat(type, out db);
        return Mathf.Pow(10, db / 20);
    }
}
