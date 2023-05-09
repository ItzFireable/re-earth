using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScrolling : MonoBehaviour
{
    // Speed multipliers
    [SerializeField] public float speedMultiplier;
    [SerializeField] public float foregroundSpeedMultiplier;

    // Background and foreground
    [SerializeField] public GameObject background;
    [SerializeField] public GameObject foreground;

    // List of copies
    List<GameObject> backgroundClones = new List<GameObject>();
    List<GameObject> foregroundClones = new List<GameObject>();

    float xOffset = 0.001f;

    void Start()
    {
        // Instantiate clones
        GameObject leftBackground = Instantiate(background, new Vector3(-18.5f,0,0), transform.rotation);
        GameObject rightBackground = Instantiate(background, new Vector3(18.5f,0,0), transform.rotation);

        GameObject leftForeground = Instantiate(foreground, new Vector3(-18.5f,0,0), transform.rotation);
        GameObject rightForeground = Instantiate(foreground, new Vector3(18.5f,0,0), transform.rotation);

        leftBackground.transform.parent = this.gameObject.transform;
        rightBackground.transform.parent = this.gameObject.transform;

        leftForeground.transform.parent = this.gameObject.transform;
        rightForeground.transform.parent = this.gameObject.transform;

        // Add them to a list
        backgroundClones.Add(background);
        backgroundClones.Add(leftBackground);
        backgroundClones.Add(rightBackground);

        foregroundClones.Add(foreground);
        foregroundClones.Add(leftForeground);
        foregroundClones.Add(rightForeground);
    }

    void Update()
    {
        // Go through every background
        foreach (GameObject bg in backgroundClones)
        {
            // Update background position
            bg.transform.localPosition = new Vector3(bg.transform.localPosition.x - (xOffset * speedMultiplier),0,0);

            // If offscreen, move it to the right
            if (bg.transform.localPosition.x <= -18.5f)
                bg.transform.localPosition = new Vector3(18.5f,0,0);
        }

        // Go through every foreground
        foreach (GameObject fg in foregroundClones)
        {
            // Update foreground position
            fg.transform.localPosition = new Vector3(fg.transform.localPosition.x - (xOffset * speedMultiplier * foregroundSpeedMultiplier),0,0);

            // If offscreen, move it to the right
            if (fg.transform.localPosition.x <= -18.5f)
                fg.transform.localPosition = new Vector3(18.5f,0,0);
        }
    }
}
