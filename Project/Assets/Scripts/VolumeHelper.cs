using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeHelper : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private VolumeSettings settings;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = (settings.GetVolume(type) * 100).ToString("0") + "%";
        GetComponent<Slider>().value = settings.GetVolume(type);
    }

    public void SetVolume(float volume)
    {
        settings.SetVolume(type, volume);
        text.text = (volume * 100).ToString("0") + "%";
    }
}
