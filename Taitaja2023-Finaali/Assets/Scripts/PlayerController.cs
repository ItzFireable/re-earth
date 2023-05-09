using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parts of the movement taken from https://github.com/KimiJok1/Taitaja-2023-Peliprojekti/blob/main/Assets/Scripts/PlayerController.cs
public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidBody;

    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;

    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;

    [SerializeField] private bool isGrounded = true;

    private float targetDirection = 1;
    [SerializeField] GameObject hitbox;

    IEnumerator ShowHitbox()
    {
        hitbox.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        hitbox.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Attack(string type)
    {
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
        
    }

    void Update()
    {
        // Get inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Get direction multiplier
        targetDirection = horizontalInput < 0 ? -1 : 1;

        // Set RigibBodys velocity based on input and speed multiplier
        rigidBody.velocity = new Vector2(Mathf.Abs(horizontalInput) * speedMultiplier * targetDirection, rigidBody.velocity.y);

        // If spacebar is pressed and player is grounded, set player's Y velocity to up direction multiplied by jump multiplier
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            rigidBody.velocity = Vector3.up * jumpMultiplier;
        }

        // If clicked, call for attack (normal)
        if (Input.GetMouseButtonDown(0))
            Attack("Normal");

        // If right clicked, call for attack (heavy)
        if (Input.GetMouseButtonDown(1))
            Attack("Heavy");
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
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
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
        }
    }
}
