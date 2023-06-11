using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterParallax : MonoBehaviour
{
    private float length, startpos, timeElapsed;
    [SerializeField] private  Transform cam;
    [Range(0, 1)]
    [SerializeField] private float parallexEffect;
    [SerializeField] private float speedMultiplier;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }
    void Update() {
        timeElapsed += speedMultiplier * Time.deltaTime;

        float temp = (cam.position.x * (1 - parallexEffect)) - timeElapsed;
        float dist = (cam.position.x * parallexEffect) + timeElapsed;
        // transform.position += new Vector3(speedMultiplier * Time.deltaTime + startpos + dist, 0, 0);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);


        if (temp > startpos + length) {
            startpos += length + timeElapsed;
            timeElapsed = 0;
        }
        else if (temp < startpos - length) { 
            startpos -= length - timeElapsed;
            timeElapsed = 0;
        }
    }

}
