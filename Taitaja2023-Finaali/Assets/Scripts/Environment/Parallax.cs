using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Script taken from https://vionixstudio.com/2022/09/22/unity-parallax-background/
    [SerializeField] private GameObject object;
    [SerializeField] private Camera camera;
    [SerializeField] private float parallaxValue;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = object.transform.position;      
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = camera.transform.position * parallaxValue;
        relative_pos.z = startPosition.z;
        object.transform.position = startPosition + relative_pos;
    }
}
