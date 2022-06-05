using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator playerAnim;
    [SerializeField] Transform playerTransform;
    [SerializeField] Rigidbody playerRb;

    float horizontalInput;
    float verticalInput;

    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

        playerRb.MovePosition(playerTransform.position + speed * Time.deltaTime * direction);

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        ManageAnimations();
    }

    void ManageAnimations()
    {
        if (Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput) > 0)
        {
            if (playerAnim.GetFloat("Speed_f") != speed)
            {
                playerAnim.SetFloat("Speed_f", speed);
            }
        }
        else
        {
            if (playerAnim.GetFloat("Speed_f") != 0)
            {
                playerAnim.SetFloat("Speed_f", 0);
            }
        }
    }
}
