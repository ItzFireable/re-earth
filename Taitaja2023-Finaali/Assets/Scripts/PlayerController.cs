using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parts of the movement taken from https://github.com/KimiJok1/Taitaja-2023-Peliprojekti/blob/main/Assets/Scripts/PlayerController.cs
public class PlayerController : MonoBehaviour
{
    // Serialized objects for player
    [SerializeField] GameObject hitbox;
    [SerializeField] Rigidbody2D rigidBody;

    // Serialized movement variables
    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float jumpLimit;

    // Inputs
    private float verticalInput;
    private float horizontalInput;

    // Movement variables
    private int jumpCount = 0;
    private bool isRunning = false;
    private bool isFalling = false;
    private bool isGrounded = false;
    private bool isOnCooldown = false;
    private float targetDirection = 1;

    // Objects for player
    private Animator animator;
    private SpriteRenderer sprRenderer;

    // Sound manager
    private PlayerSoundManager soundManager;

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

    // Used for managing attacks
    IEnumerator Attack()
    {
        // Randomize animation
        int anim = Random.Range(1,4);

        // Enable cooldown, play sound
        isOnCooldown = true;
        soundManager.PlaySound("Attack",anim);

        // (Debugging) show hitbox
        hitbox.GetComponent<SpriteRenderer>().enabled = true;
        hitbox.transform.localPosition = new Vector3(1*targetDirection,0,0);

        // Play animation and wait for it to finish
        animator.SetTrigger("Attack" + anim);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Disable cooldown, (debugging) hide hitbox
        isOnCooldown = false;
        hitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Called before attack to check attack type & to check cooldown
    void CheckAttack(string type)
    {
        if (isOnCooldown) return;

        switch(type)
        {
            case "Normal":
                StartCoroutine("Attack");
                break;
            case "Heavy":
                // StartCoroutine("HeavyAttack");
                break;
        }
    }

    void Start()
    {
        // Get sprite animator, sprite renderer and sound manager
        animator = GetComponent<Animator>();
        sprRenderer = GetComponent<SpriteRenderer>();
        soundManager = GetComponent<PlayerSoundManager>();
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

        // Set running on animator if player is moving (TODO: Change this)
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
            CheckAttack("Normal");

        // If right clicked, call for attack (heavy)
        if (Input.GetMouseButtonDown(1))
            CheckAttack("Heavy");

        if (Input.GetKeyDown(KeyCode.LeftControl))
            print("block");
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
}
