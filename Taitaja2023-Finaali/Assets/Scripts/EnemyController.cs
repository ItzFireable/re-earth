using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;

    [SerializeField] private int maxHealth;
    private int health;
    private bool facing;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        distanceToPlayer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void distanceToPlayer()
    {
        float difference = transform.position.x - player.transform.position.x;
        if(difference < 0 && facing)
        {
            transform.position = new Vector3(transform.position.x + 1.25f, transform.position.y, 0f);
            
        }
        else if(difference > 0 && !facing)
        {
            transform.position = new Vector3(transform.position.x - 1.25f, transform.position.y, 0f);
            
        }
    }
        
}
