using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttack : MonoBehaviour
{
    [SerializeField] private Vector2 hitBoxCenter = new Vector2(1.5f, 1.5f);
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1.5f, 1.5f);
    [SerializeField] private float damage = 10f;
    [SerializeField] private float progressSpeed = 1f;
    private Animator animator;
    private float progress = 0f;
    float motionTime = 0.8f;
    bool disabled = false;

    void Start() => animator = GetComponent<Animator>();

    public void Disable(bool disabled) => this.disabled = disabled;

    // very very scuffed
    void Update()
    {
        if (disabled) return;
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            animator.SetTrigger("HeavyAttack");
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            animator.SetBool("HeavyAttackHeld", true);
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("HA-Stages"))
                progress += progressSpeed * Time.deltaTime;
            if(progress > 1) progress = 1;
            animator.SetFloat("HeavyAttackTime", (progress / 1) * motionTime);
        } else
        {
            animator.SetBool("HeavyAttackHeld", false);
        }

    }

    void Freeze() { 
        print("freeze");
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<PlayerController>().isOnCooldown = true;
        GetComponent<PlayerController>().isAttacking = true;    
        }
    public void UnFreeze() { 
        print("unfreeze");
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<PlayerController>().isOnCooldown = false;
        GetComponent<PlayerController>().isAttacking = false;
        }   

    void StartTrigger() => progress = 0f;
    void HitTrigger() 
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)hitBoxCenter, hitBoxSize, 0f);
        print("hit");

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                print("hit enemy for " + damage * (progress / 1) + " damage");
                collider.gameObject.GetComponent<EnemyController>().TakeDamage(damage * (progress / 1));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)hitBoxCenter, hitBoxSize);
    }
}
