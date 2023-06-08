using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Script taken from https://vionixstudio.com/2022/09/22/unity-parallax-background/
    [SerializeField] private GameObject sky;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float parallaxValue;

    // Start position for parallax effect
    private Vector3 startPosition;

    void Start()
    {
        // Get starting position
        startPosition = sky.transform.position;      
    }

    void Update()
    {
        // Get relative position with parallax multiplier
        Vector3 relative_pos = targetCamera.transform.position * parallaxValue;
        relative_pos.z = startPosition.z;

        // Update the sky transform to position correctly with the relative position
        sky.transform.position = startPosition + relative_pos;
    }
}
