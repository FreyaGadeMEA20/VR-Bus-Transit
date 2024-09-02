using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TempPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of player movement
    public float rotationSpeed = 100f; // Speed of player rotation
    private Rigidbody rb; // Rigidbody component of the player

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input from the horizontal and vertical axis
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Apply movement to the player's rigidbody
        rb.velocity = transform.TransformDirection(movement) * moveSpeed;

        // Rotate the player based on horizontal input
        transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime * Input.GetAxis("Mouse X"));
    }
}
