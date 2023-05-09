using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parts of the movement taken from https://github.com/KimiJok1/Taitaja-2023-Peliprojekti/blob/main/Assets/Scripts/PlayerController.cs
public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject hitbox;
    [SerializeField] Rigidbody2D rigidBody;

    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float jumpLimit;

    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;

    private int jumpCount = 0;
    private bool isRunning = false;
    private bool isFalling = false;
    private bool isGrounded = false;
    private bool isOnCooldown = false;
    private float targetDirection = 1;

    private Animator animator;
    private SpriteRenderer sprRenderer;

    float GetAnimationLength(string name)
    {
        var animController = animator.runtimeAnimatorController;
        var clips = animController.animationClips;

        foreach (AnimationClip clip in clips)
            if (clip.name == name) return clip.length;

        return 1;
    }

    IEnumerator ShowHitbox()
    {
        int anim = Random.Range(1,4);
        float timer = GetAnimationLength("Attack" + anim);
        
        isOnCooldown = true;
        hitbox.GetComponent<SpriteRenderer>().enabled = true;
        animator.SetTrigger("Attack" + anim);

        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        isOnCooldown = false;
        hitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Attack(string type)
    {
        if (isOnCooldown) return;

        StartCoroutine("ShowHitbox");
        switch(type)
        {
            case "Normal":
                break;
            case "Heavy":
                break;
        }
    }

    void Start()
    {
        sprRenderer = sprite.GetComponent<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
    }

    void Update()
    {
        // Get inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0)
        {
            // Get direction multiplier
            targetDirection = horizontalInput < 0 ? -1 : 1;
            sprRenderer.flipX = targetDirection == 1 ? false : true;
        }

        // Set RigibBodys velocity based on input and speed multiplier
        rigidBody.velocity = new Vector2(Mathf.Abs(horizontalInput) * speedMultiplier * targetDirection, rigidBody.velocity.y);

        /*if (!isRunning && Input.GetAxisRaw("Horizontal") != 0)
        {
            isRunning = true;
            print("Started running");
        }
        else if (isRunning && Input.GetAxisRaw("Horizontal") == 0)
        {
            isRunning = false;
            print("Stopped running");
        }*/

        animator.SetBool("Running",rigidBody.velocity.x != 0);

        // If spacebar is pressed and player is grounded, set player's Y velocity to up direction multiplied by jump multiplier
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < jumpLimit))
        {
            jumpCount++;
            isGrounded = false;
            rigidBody.velocity = Vector3.up * jumpMultiplier;
        }

        // If clicked, call for attack (normal)
        if (Input.GetMouseButtonDown(0))
            Attack("Normal");

        // If right clicked, call for attack (heavy)
        if (Input.GetMouseButtonDown(1))
            Attack("Heavy");

        if (Input.GetKeyDown(KeyCode.LeftControl))
            print("block");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Compare tag to see what player is colliding with
        if (collision.gameObject.CompareTag("Ground")) {
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
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
    }
}
