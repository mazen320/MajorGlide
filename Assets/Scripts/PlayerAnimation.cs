using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public float followAniamtionBlendTime;
    public float aimAniamtionBlendTime;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (playerController.moving)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }

        // Follow Animation
        if (playerController.jumping)
        {
            animator.SetBool("Jumping", true);
            animator.SetBool("Grounded", false);
            animator.SetBool("Falling", false);
        }
        if (playerController.playerVelocity.y < 0f)
        {
            animator.SetBool("Falling", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Grounded", false);
        }
        if (playerController.grounded)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);

            if (!playerController.jumping)
            {
                animator.SetFloat("Speed", playerController.move.magnitude, followAniamtionBlendTime, Time.deltaTime);
                animator.SetBool("Jumping", false);
                animator.SetBool("Falling", false);
            }
        }

        // Aiming Animation
        if (playerController.aiming)
        {
            animator.SetBool("Aiming", true);
            animator.SetFloat("Input X", playerController.horizontalInput, aimAniamtionBlendTime, Time.deltaTime);
            animator.SetFloat("Input Y", playerController.verticalInput, aimAniamtionBlendTime, Time.deltaTime);
        }
        else
        {
            animator.SetBool("Aiming", false);
        }

        // Gliding Animation
        if (playerController.gliding)
        {
            animator.SetFloat("Speed", playerController.move.magnitude, followAniamtionBlendTime, Time.deltaTime);
            animator.SetBool("Gliding", true);
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
        }
        else
        {
            animator.SetBool("Gliding", false);
        }
    }
}
