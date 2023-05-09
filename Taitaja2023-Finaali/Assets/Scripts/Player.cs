using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidBody;

    [SerializeField] float speedMultiplier;
    [SerializeField] float jumpMultiplier;

    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;

    private bool isGrounded = true;

    void Start()
    {
        
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        float dir = horizontalInput < 0 ? -1 : 1;
        rigidBody.velocity = new Vector2(Mathf.Abs(horizontalInput) * speedMultiplier * dir, rigidBody.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            rigidBody.velocity = Vector3.up * jumpMultiplier;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
}
