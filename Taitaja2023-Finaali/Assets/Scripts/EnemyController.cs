using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Serialized fields for the enemy
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject realPosition;

    // Health properties
    [SerializeField] private int maxHealth;
    private int health;

    // Facig direction
    private bool facing;

    // Start is called before the first frame update
    void Start()
    {
        // Get health & player
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Update facing direction
        faceToPlayer();
    }

    // Function to check if enemy is facing player or not
    void faceToPlayer()
    {
        // Get positional difference for player and enemy
        float difference = realPosition.transform.position.x - player.transform.position.x;

        // Checks if it is already facing player
        if((difference < 0 && facing) || (difference > 0 && !facing))
        {
            // Flip enemy and its position
            transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
            if(facing)
                transform.position = new Vector3(transform.position.x + 1.75f, transform.position.y, 0f);
            else
                transform.position = new Vector3(transform.position.x - 1.75f, transform.position.y, 0f);

            // Flips the facing direction
            facing = !facing;
        }
    }
        
}
