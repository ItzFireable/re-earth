using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

// Parts of the movement taken from https://github.com/KimiJok1/Taitaja-2023-Peliprojekti/blob/main/Assets/Scripts/PlayerController.cs
public class PlayerController : MonoBehaviour
{
    // Serialized objects for player
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject hitbox;
    [SerializeField] GameObject cameraPoint;
    [SerializeField] Rigidbody2D rigidBody;

    // Serialized movement variables
    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float jumpLimit;

    // Energy variables
    [SerializeField] public float energy = 100;
    [SerializeField] public float maxEnergy = 100;

    // Energy multipliers
    [SerializeField] public float energyGainMultiplier = 1;
    [SerializeField] public float maxEnergyMultiplier = 1;

    // Inputs
    private float verticalInput;
    private float horizontalInput;

    // Movement variables
    private int jumpCount = 0;
    private bool isDead = false;
    private bool isFalling = false;
    private bool isGrounded = false;
    private bool isAttacking = false;
    private bool isOnCooldown = false;
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
        // Freeze player, set to dead and run death animation
        isDead = true;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Die");

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
        soundManager.PlaySound("Attack",anim);

        // Freeze player position
        rigidBody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        // Play animation and wait for it to finish
        animator.SetTrigger("Attack" + anim);
        

        // Wait until animation starts
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0);

        // Enable hitbox collision check
        isAttacking = true;
        hitbox.transform.localPosition = new Vector3((1.5f*targetDirection),-0.95f,0);

        hitbox.GetComponent<BoxCollider2D>().enabled = true;

        // Wait for the animation length
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        hitbox.GetComponent<BoxCollider2D>().enabled = false;
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
        energy += (amount * energyGainMultiplier);
        
        // Energy limit
        if(energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    public void TakeDamage(float amount, bool playAnim)
    {
        if (isDead || dashing) return;

        // Run take damage animation and remove energy
        if (playAnim)
        {
            animator.SetTrigger("TakeDamage");
            soundManager.PlaySound("Damage");
        }

        energy -= amount;

        // Energy limit
        if(energy < 0)
        {
            energy = 0;
        }
    }

    void DamageOverTime()
    {
        energy -= 0.5f;
    }

    void Start()
    {
        // Get sprite animator, sprite renderer and sound manager
        soundManager = GetComponent<PlayerSoundManager>();

        // Take damage over time
        InvokeRepeating("DamageOverTime", 1f, 0.5f);

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
        verticalInput = Input.GetAxis("Vertical");

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

            if (rigidBody.velocity.x != 0 && !isAttacking)
                soundManager.PlaySound("Run");
            else
                soundManager.StopSound("Run");

            // Set running on animator if player is moving (TODO: Change this)
            animator.SetBool("Running",rigidBody.velocity.x != 0 && !isAttacking);

            if(Input.GetKeyDown(KeyCode.Space))
                Dash();
            
            // If clicked, call for attack (normal)
            if (Input.GetMouseButtonDown(0))
                CheckAttack("Normal");

            if (Input.GetKeyDown(KeyCode.LeftControl))
                print("block");
        }
        else if(dashing && !isDead)
        {
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
            if(playerPos != dashPoint)
                transform.position = Vector2.MoveTowards(transform.position, dashPoint, dashSpeed * Time.deltaTime);
            else
                dashing = false;
        }

        // Update UI
        Transform EnergyBar = canvas.transform.Find("EnergyBar");
        EnergyBar.GetComponent<Slider>().value = (float)System.Math.Round(energy,0);
        EnergyBar.GetComponent<Slider>().maxValue = (float)System.Math.Round(maxEnergy * maxEnergyMultiplier,0);
        EnergyBar.Find("FillText").GetComponent<TMP_Text>().text = "Energy: " + System.Math.Round(energy,0) + "/" + System.Math.Round(maxEnergy * maxEnergyMultiplier,0);

        // Check player health
        if (System.Math.Round(energy,0) <= 0)
        {
            energy = 0;
            if (isDead) return;
            StartCoroutine("Death");
        }
    }

    void Dash()
    {
        dashing = true;
        animator.SetTrigger("Dash");
        soundManager.PlaySound("Dash");

        if((transform.position.x + (dashDistance * targetDirection)) < maxX && (transform.position.x + (dashDistance * targetDirection)) > minX)
        {
            dashPoint = new Vector2(transform.position.x + dashDistance * targetDirection, transform.position.y);
        }
        else if((transform.position.x + (dashDistance * targetDirection)) > maxX){
            dashPoint = new Vector2(maxX - 0.5f, transform.position.y);
        }
        else if((transform.position.x + (dashDistance * targetDirection)) < minX){
            dashPoint = new Vector2(minX + 0.5f, transform.position.y);
        }
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
        Gizmos.DrawWireCube(hitbox.transform.position, new Vector3(2, 0.5f, 1));
    }
}
