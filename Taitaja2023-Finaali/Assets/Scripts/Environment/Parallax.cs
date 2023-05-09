using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Script taken from https://vionixstudio.com/2022/09/22/unity-parallax-background/
    [SerializeField] private GameObject sky;
    [SerializeField] private Camera your_camera;
    [SerializeField] private float parallax_value;

    private Vector3 startposition;

    // Start is called before the first frame update
    void Start()
    {
        startposition = sky.transform.position;      
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = your_camera.transform.position * parallax_value;
        relative_pos.z = startposition.z;
        sky.transform.position = startposition + relative_pos;
    }
}
