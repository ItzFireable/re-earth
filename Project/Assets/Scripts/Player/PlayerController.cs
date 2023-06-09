using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using System;

// Parts of the movement taken from https://github.com/KimiJok1/Taitaja-2023-Peliprojekti/blob/main/Assets/Scripts/PlayerController.cs
public class PlayerController : MonoBehaviour
{
    // Serialized objects for player
    [SerializeField] Transform hitbox;
    [SerializeField] private Transform center;
    [SerializeField] GameObject cameraPoint;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayer;

    // Events
    [SerializeField] private UnityEvent onInterrupted;
    [SerializeField] private UnityEvent onDeath;

    // Serialized movement variables
    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float jumpLimit;

    // Inputs
    private float verticalInput;
    private float horizontalInput;

    // Movement variables
    private int jumpCount = 1;
    public bool isDead { get; private set; } = false;
    private bool isFalling = false;
    private bool isGrounded = false;
    public bool isAttacking = false;
    public bool isOnCooldown = false;
    private float targetDirection = 1;

    private bool dashing = false;
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashSpeed = 10f;

    Vector2 dashPoint;
    public float minX, maxX;
    [SerializeField] private GameObject cameraBoundaries;

    // Objects for player
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprRenderer;
    [SerializeField] private ParticleSystem dust;

    // Sound manager
    private PlayerSoundManager soundManager;

    // Start time
    float startTime;

    [SerializeField] private float damage = 1f;

    // Unused function to get animation length of any animation from the controller. Will probably be used someday.
    float GetAnimationLength(string name)
    {
        var animController = animator.runtimeAnimatorController;
        var clips = animController.animationClips;

        // Go through all clips and find the correct clip.
        foreach (AnimationClip clip in clips)
            if (clip.name == name) return clip.length;

        return 1;
    }

    IEnumerator Death()
    {
        onDeath.Invoke();
        // Freeze player, set to dead and run death animation
        isDead = true;
        soundManager.StopSound("Run");
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Die");
        ToggleDust(false);

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("MainMenu");
    }

    // Used for managing attacks
    IEnumerator Attack()
    {
        // Randomize animation
        int anim = UnityEngine.Random.Range(1,4);

        // Enable cooldown, play sound
        isOnCooldown = true;
        soundManager.PlayOneShot("Attack"+anim);

        // Freeze player position
        rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        // Play animation and wait for it to finish
        animator.SetTrigger("Attack" + anim);
        

        // Wait until animation starts
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0);

        // Enable hitbox collision check
        isAttacking = true;
        hitbox.localPosition = new Vector3((1.5f*targetDirection),-0.95f,0);


        // Wait for the animation length
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        
        animator.SetBool("Attacking", false);

        // Unfreeze player position
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Disable hitbox collision check
        isAttacking = false;

        // (debugging) hide hitbox, wait half a second
        yield return new WaitForSeconds(0.25f);

        // Disable cooldown
        isOnCooldown = false;
    }

    void SetAttackTrigger() => hitbox.GetComponent<BoxCollider2D>().enabled = true;
    void SetAttackTriggerFinish() => hitbox.GetComponent<BoxCollider2D>().enabled = false;

    // Called before attack to check attack type & to check cooldown
    void CheckAttack(string type)
    {
        if (isOnCooldown) return;
        if (isDead) return;

        switch(type)
        {
            case "Normal":
                StartCoroutine("Attack");
                break;
        }
    }

    public void GainEnergy(float amount)
    {
        if (isDead) return;

        // Add energy
        // energy += (amount * energyGainMultiplier);

        // TODO: Change this to work without getting components
        GetComponent<PlayerEnergy>().GainEnergy(amount);
        
        // Energy limit
        // if(energy > maxEnergy)
        // {
        //     energy = maxEnergy;
        // }
    }

    public void TakeDamage(float amount, bool playAnim)
    {
        if (isDead || dashing) return;

        // Run take damage animation and remove energy
        if (playAnim)
        {
            animator.SetTrigger("TakeDamage");
            soundManager.PlayOneShot("Damage");
            onInterrupted.Invoke();
        }

        // energy -= amount;

        // TODO: Change this to work without getting components
        GetComponent<PlayerEnergy>().LoseEnergy(amount);

        // Energy limit
        // if(energy < 0)
        // {
        //     energy = 0;
        // }
    }

    // void DamageOverTime()
    // {
    //     energy -= 1f;
    // }

    void Start()
    {
        // Get sprite animator, sprite renderer and sound manager
        soundManager = GetComponent<PlayerSoundManager>();

        // Take damage over time
        // InvokeRepeating("DamageOverTime", 1f, 1f);

        // Get start time
        startTime = Time.time;

        // Compare the points
        foreach (var i in cameraBoundaries.GetComponent<PolygonCollider2D>().points)
        {
            if (i.x < minX) 
                minX = i.x;
            if (i.x > maxX)
                maxX = i.x;
        }
            
    }

    void Update()
    {
        if(Time.timeScale == 0)
            return;
        // Get inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // verticalInput = Input.GetAxis("Vertical");

        // Run these if player isn't dead
        if (!isDead && !dashing)
        {
            if (horizontalInput != 0)
            {
                // Get direction multiplier
                targetDirection = horizontalInput < 0 ? -1 : 1;
                sprRenderer.flipX = targetDirection == 1 ? false : true;
            }

            // Set RigibBodys velocity based on input and speed multiplier
            if(!isAttacking)
                rigidBody.velocity = new Vector2(Mathf.Abs(horizontalInput) * speedMultiplier * targetDirection, rigidBody.velocity.y);

            // Run dust particle system & audio if player is moving
            if(rigidBody.velocity.x != 0 && !isAttacking && isGrounded){
                soundManager.PlaySound("Run", loop: true);
                ToggleDust(true);
            }
            else if(rigidBody.velocity.x == 0 || isAttacking || !isGrounded){
                ToggleDust(false);
                soundManager.StopSound("Run");
            }
                
                

            // Set running on animator if player is moving (TODO: Change this)
            animator.SetBool("Running",rigidBody.velocity.x != 0 && !isAttacking);

            if(Input.GetKeyDown(KeyCode.Space))
                Dash();

            if(Input.GetKeyDown(KeyCode.W))
                Jump();
            
            // If clicked, call for attack (normal)
            if (Input.GetMouseButtonDown(0))
                CheckAttack("Normal");

            if (Input.GetKeyDown(KeyCode.LeftControl))
                print("block");
        }
        else if(dashing && !isDead)
        {
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
            bool facingWall = Physics2D.Raycast(playerPos, center.right * targetDirection, 0.5f, groundLayer);
            if(playerPos != dashPoint && !facingWall)
                transform.position = Vector2.MoveTowards(transform.position, dashPoint, dashSpeed * Time.deltaTime);
            else
                dashing = false;
        }

        // Update UI
        // Transform EnergyBar = canvas.transform.Find("EnergyBar");
        // EnergyBar.GetComponent<Slider>().value = (float)System.Math.Round(energy,0);
        // EnergyBar.GetComponent<Slider>().maxValue = (float)System.Math.Round(maxEnergy * maxEnergyMultiplier,0);
        // EnergyBar.Find("FillText").GetComponent<TMP_Text>().text = "Energy: " + Mathf.Round(energy) + "/" + Mathf.Round(maxEnergy * maxEnergyMultiplier);

        // Check player health
        // if (System.Math.Round(energy,0) <= 0)
        // {
        //     energy = 0;
        //     if (isDead) return;
        //     StartCoroutine("Death");
        // }
    }

    void ToggleDust(bool toggle){
        var emission = dust.emission;

        if(emission.enabled != toggle)
            emission.enabled = toggle;
    }

    void Jump()
    {
        if (isDead) return;

        // Check if player is grounded
        if (isGrounded)
        {
            // Set animator trigger and play sound
            // animator.SetTrigger("Jump");
            // soundManager.PlaySound("Jump");

            // Add force to player
            rigidBody.AddForce(new Vector2(0, jumpMultiplier), ForceMode2D.Impulse);
        }
    }

    void Dash()
    {
        dashing = true;
        animator.SetTrigger("Dash");
        soundManager.PlayOneShot("Dash");
        onInterrupted.Invoke();


        dashPoint = new Vector2(transform.position.x + dashDistance * targetDirection, transform.position.y);

        // else if((transform.position.x + (dashDistance * targetDirection)) > maxX){
        //     dashPoint = new Vector2(maxX - 0.5f, transform.position.y);
        // }
        // else if((transform.position.x + (dashDistance * targetDirection)) < minX){
        //     dashPoint = new Vector2(minX + 0.5f, transform.position.y);
        // }
    }

    void CancelDash()
    {
        dashing = false;
        dashPoint = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Compare tag to see what player is colliding with
        if (collision.gameObject.CompareTag("Ground")) 
        {
            bool checkDir = true;

            // Get all contact points for object
            ContactPoint2D[] allPoints = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(allPoints);

            // Compare the points
            foreach (var i in allPoints)
                if (i.point.y > transform.position.y) 
                    checkDir = false;

            // Enable jumping again
            isGrounded = checkDir == true ? checkDir : isGrounded;
            jumpCount = isGrounded ? 0 : jumpCount;
            isFalling = isGrounded ? true : isFalling;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Compare tag to see what player is colliding with
        if (collision.gameObject.CompareTag("Ground")) 
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(hitbox.position, new Vector3(2, 0.5f, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * targetDirection * 1.5f);
    }
}
