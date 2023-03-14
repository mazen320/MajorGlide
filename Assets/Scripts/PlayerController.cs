using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Player
    [Header("Player")]
    public CharacterController controller;
    public Camera mainCamera;
    public Transform head;

    // Camera
    [Header("Camera")]
    public CinemachineVirtualCamera followCam;
    public CinemachineVirtualCamera aimCam;
    public CinemachineVirtualCamera glidingCam;

    // Health
    [Header("Health")]
    public PlayerHealth playerHealth;

    // Movement
    [Header("Movement")]
    public float moveSpeed;

    public float horizontalInput;
    public float verticalInput;
    public Vector3 move;
    public Vector3 playerVelocity;
    public bool moving;
    public bool grounded;

    // Jump
    [Header("Jump")]
    public float jumpHeight;
    public float normalGravityValue;
    public float fallMultiplier;
    public bool jumping;

    // Rotation
    [Header("Rotation")]
    public Transform orientation;
    public float followRotationSpeed;
    public float aimRotationSpeed;

    // Aim
    [Header("Aiming")]
    public Rig aimRig;
    public Transform aimBall;
    public LayerMask aimLayerMask;
    public LineRenderer aimLine;
    public Vector3 aimLineOffset;
    public bool aiming;

    // Shoot
    [Header("Shooting")]
    public float laserDamage;
    public float laserRange;
    public LineRenderer laserLine;
    public Light laserImpactLight;
    public GameObject sparksPartSys;
    public bool shooting;
    private RaycastHit hit;

    // Gliding
    [Header("Gliding")]
    public float glidingSpeed;
    public float glidingHeight;
    public float glidingYOffset;
    public LayerMask glidingLayerMask;
    public List<GameObject> trails;
    public Transform glidingCheck1;
    public Transform glidingCheck2;
    public bool canGlide;
    public bool canInitJump;
    public bool gliding;
    private bool wasGliding;
    public float glideEnergy = 100f;
    public float glideEnergyDepletionRate = 10f;
    public float glideEnergyRegenRate = 5f;
    public Slider glideEnergySlider;

    //Animation
    [Header("Animation")]
    public PlayerAnimation animator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // move
        MovePlayer();

        if (move.magnitude != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        // player rotation
        RotatePlayer();

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Jump();
        }

        // aim
        if (Input.GetMouseButton(1) && !gliding)
        {
            StartAim();
        }
        else
        {
            CancelAim();
        }

        // shoot
        if (Input.GetMouseButton(0) && aiming)
        {
            if (!shooting)
            {
                Instantiate(sparksPartSys, aimBall);
            }

            Shoot();
        }
        else
        {
            laserLine.gameObject.SetActive(false);

            shooting = false;
            laserImpactLight.intensity = 0f;
        }

        CheckGrounded();

        if (gliding)
        {
            glideEnergy -= Time.deltaTime * glideEnergyDepletionRate;
            glideEnergySlider.value = glideEnergy;

            if (glideEnergy <= 0)
            {
                glideEnergy = 0;
                canInitJump = false;
                gliding = false;
                canGlide = false;
                playerVelocity += transform.up * normalGravityValue * fallMultiplier;
            }
        }

        // realistic fall
        if (!grounded && playerVelocity.y < 0 && !gliding)
        {
            playerVelocity += transform.up * normalGravityValue * fallMultiplier;
        }
    }

    private void MovePlayer()
    {
        // move input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        move = horizontalInput * orientation.right + verticalInput * orientation.forward;
        move.y = 0;

        // apply movement
        if (!gliding)
        {
            controller.Move(Vector3.ClampMagnitude(move, 1f) * moveSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(orientation.forward * glidingSpeed * Time.deltaTime);
            glidingCam.gameObject.SetActive(true);
        }

        // if able to glide Check
        if (!Physics.Raycast(transform.position, -transform.up, glidingHeight, glidingLayerMask))
        {
            canGlide = true;
        }
        else if (Physics.Raycast(transform.position, -transform.up, 3f, glidingLayerMask))
        {
            canGlide = false;
        }

        if (!Physics.Raycast(glidingCheck1.position, -glidingCheck1.up, glidingHeight, glidingLayerMask) || !Physics.Raycast(glidingCheck2.position, -glidingCheck2.up, glidingHeight, glidingLayerMask))
        {
            canInitJump = true;
        }
        else if (Physics.Raycast(glidingCheck1.position, -glidingCheck1.up, 10f, glidingLayerMask) && Physics.Raycast(glidingCheck2.position, -glidingCheck2.up, 10f, glidingLayerMask))
        {
            canInitJump = false;
        }

        // gliding
        if (playerVelocity.y <= 0 && !grounded && canGlide  && glideEnergy > 0)
        {
            playerVelocity.y = 0f;

            // enable gliding trails
            foreach (GameObject trail in trails)
            {
                trail.SetActive(true);
            }

            gliding = true;
            jumping = false;
            wasGliding = true;
        }
        // init jump
        else if (canInitJump && grounded && moving)
        {
            Jump();
            glidingCam.gameObject.SetActive(true);
        }
        // normal
        else
        {
            playerVelocity.y += normalGravityValue * Time.deltaTime;
            glidingCam.gameObject.SetActive(false);
            gliding = false;

            // disable gliding trails
            foreach (GameObject trail in trails)
            {
                trail.SetActive(false);
            }
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        if (aiming)
        {
            RotateAiming();
        }
        else if (gliding)
        {
            RotateGliding();
        }
        else
        {
            RotateFollowing();
        }
    }

    private void RotateFollowing()
    {
        Vector3 viewDir = (transform.position - new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z)).normalized;
        orientation.forward = viewDir;

        Vector3 inputDir = ((orientation.forward * verticalInput) + (orientation.right * horizontalInput)).normalized;

        transform.forward = Vector3.Slerp(transform.forward, inputDir, followRotationSpeed * Time.deltaTime);

        if (wasGliding)
        {
            transform.forward = orientation.forward;
            wasGliding = false;
        }
    }

    private void RotateAiming()
    {
        Vector3 aimTarget = aimBall.transform.position;
        aimTarget.y = transform.position.y;
        Vector3 viewDir = (aimTarget - transform.position).normalized;
        orientation.forward = viewDir;

        transform.forward = Vector3.Slerp(transform.forward, viewDir, aimRotationSpeed * Time.deltaTime);
    }

    private void RotateGliding()
    {
        Vector3 viewDir = (transform.position - new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + glidingYOffset, mainCamera.transform.position.z)).normalized;
        orientation.forward = viewDir;

        transform.forward = Vector3.Slerp(transform.forward, orientation.forward, followRotationSpeed * Time.deltaTime);
    }


    private void Jump()
    {
        // change height of player
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * normalGravityValue);
        jumping = true;
    }

    private void StartAim()
    {
        aimCam.gameObject.SetActive(true);
        aimRig.weight = 1f;

        aimLine.gameObject.SetActive(true);

        RepositionAimBall();

        aimLine.SetPosition(0, head.position + aimLineOffset);
        aimLine.SetPosition(1, aimBall.position);

        aiming = true;
    }

    private void CancelAim()
    {
        aimCam.gameObject.SetActive(false);
        aimRig.weight = 0f;

        aimLine.gameObject.SetActive(false);

        aiming = false;
    }

    private void Shoot()
    {
        var targetHit = RepositionAimBall();

        if (targetHit && hit.transform.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            hit.transform.GetComponentInParent<EnemyManager>().TakeDamage(laserDamage);
        }

        laserLine.gameObject.SetActive(true);
        laserImpactLight.intensity = 4.5f;
        aimLine.gameObject.SetActive(false);

        laserLine.SetPosition(0, head.position + aimLineOffset);
        laserLine.SetPosition(1, aimBall.position);

        shooting = true;
    }

    private bool RepositionAimBall()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out hit, 999f, aimLayerMask))
        {
            aimBall.position = hit.point;
            return true;
        }
        else
        {
            aimBall.localPosition = new Vector3(0, 0, laserRange);
            return false;
        }
    }

    private void CheckGrounded()
    {
        grounded = controller.isGrounded;
        if (grounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumping = false;
            gliding = false;
        }
    }
}
