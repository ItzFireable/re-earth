using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterParallax : MonoBehaviour
{
    private float length, startpos;
    [SerializeField] private  Transform cam;
    [SerializeField] private float parallexEffect;
    [SerializeField] private float speedMultiplier;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {

        float temp = (cam.position.x * (1 - parallexEffect));
        float dist = (cam.position.x * parallexEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }

}
