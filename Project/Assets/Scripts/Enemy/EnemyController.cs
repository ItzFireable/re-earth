using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Serialized fields for the enemy
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject realPosition;
    [SerializeField] public RoundManager roundManager;

    // positional difference for player and enemy
    float difference;

    // Health properties
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float health;
    public bool isDead = false;
    public bool wasCollected = false;

    // Facing direction
    [SerializeField] private bool facing;
    public float speed = 1.5f;

    [SerializeField] private float attackDistance = 0.75f;
    [SerializeField] private BoxCollider2D attackArea;
    [SerializeField] private bool attacking = false;
    private bool hasAttacked = false;
    [SerializeField] private float attackCooldownTime = 0.75f;
    [SerializeField] private float damage = 10f;


    // Enemy type
    [SerializeField] private int type = 1;

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
        difference = realPosition.transform.position.x - player.transform.position.x;
        
        if(player.GetComponent<PlayerController>().energy > 0 && !isDead){
            if(!attacking)
                FaceToPlayer();
            if(Mathf.Abs(difference) < attackDistance && !attacking)
            {
                animator.SetBool("Running", false);
                StartCoroutine("Attack");
            }
            else if(Mathf.Abs(difference) > attackDistance && !attacking)
            {
                Vector3 playerPos = new Vector3(player.transform.position.x, transform.position.y, 0);
                transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
                animator.SetBool("Running", true);
            }
        }
        else{
            animator.SetBool("Running", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(realPosition.transform.position, attackDistance);
    }

    // Function to check if enemy is facing player or not
    void FaceToPlayer()
    {
        // Checks if it is already facing player
        if((difference < 0 && facing) || (difference > 0 && !facing))
        {
            // Flip enemy and its position based on its type
            transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
            if (facing)
            {
                if (type == 1)
                    transform.position = new Vector3(transform.position.x + 1.75f, transform.position.y, 0f);
                else
                    transform.position = new Vector3(transform.position.x + 2.75f, transform.position.y, 0f);
            }
            else
            {
                if (type == 1)
                    transform.position = new Vector3(transform.position.x - 1.75f, transform.position.y, 0f);
                else
                    transform.position = new Vector3(transform.position.x - 2.75f, transform.position.y, 0f);
            }

            // Flips the facing direction
            facing = !facing;
        }
    }
    
    IEnumerator Attack()
    {
        attacking = true;
        animator.SetTrigger("Attack1");
        

        yield return new WaitForSeconds(attackCooldownTime);
        
        attacking = false;
        hasAttacked = false;
    }

    void AttackTrigger()
    {
        GetComponent<AudioSource>().Play(0);
        attackArea.enabled = true;
    }
    void AttackTriggerFinish() => attackArea.enabled = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && !hasAttacked && attacking)
        {
            hasAttacked = true;
            col.gameObject.GetComponent<PlayerController>().TakeDamage(damage,true);
        }
    }

    public void TakeDamage(float amount)
    {
        if(isDead) 
            return;
        health -= amount;
        if(health <= 0)
        {
            animator.SetTrigger("Death");
            isDead = true;
            
            roundManager.setKill(gameObject);
        }
        else
        {
            animator.SetTrigger("TakeDamage");
        }
    }
}
