using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject realPosition;

    [SerializeField] private int maxHealth;
    private int health;
    private bool facing;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        faceToPlayer();
    }

    void faceToPlayer()
    {
        float difference = realPosition.transform.position.x - player.transform.position.x;
        if((difference < 0 && facing) || (difference > 0 && !facing))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
            if(facing)
                transform.position = new Vector3(transform.position.x + 1.75f, transform.position.y, 0f);
            else
                transform.position = new Vector3(transform.position.x - 1.75f, transform.position.y, 0f);
            facing = !facing;
        }
    }
        
}
