using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform flashlightTransform; // Asigna esto en el Inspector
    [SerializeField] private Vector3 flashlightBobAmount = new Vector3(0.02f, 0.02f, 0f);
    [SerializeField] private float flashlightBobSpeed = 6f;

    private Vector3 flashlightOriginalPos;
    private float flashlightBobTimer = 0f;

    private Rigidbody rb;
    public AudioSource pasosAudioSource;
    public AudioSource respiracionAudioSource;
    public AudioClip caminarClip;
    public AudioClip correrClip;
    public AudioClip saltoClip;
    public AudioClip respiracionCalmadaNormal;
    public AudioClip respiracionAceleradaSprint;
    public AudioClip respiracionAceleradaCooldown;
    [SerializeField] private Vector3 standingCamPos = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private Vector3 crouchingCamPos = new Vector3(0f, 0.2f, 0f);
    [SerializeField] private float crouchTransitionSpeed = 6f;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    public bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    public bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;
    public bool isJumping = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchSpeed = 2f;

    // Internal Variables
    public bool isCrouched = false;
    private Vector3 originalScale;
    private float normalSpeed;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        crosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }

        flashlightOriginalPos = flashlightTransform.localPosition;

    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth = screenWidth * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
            {
                sprintBarCG.alpha = 0;
            }
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion

        normalSpeed = walkSpeed;
    }

    float camRotation;

    private void Update()
    {
        #region Camera

        // Control camera movement
        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
            FlashlightBob();
        }

        #region Camera Zoom

        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
            {
                if (!isZoomed)
                {
                    isZoomed = true;
                }
                else
                {
                    isZoomed = false;
                }
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if (holdToZoom && !isSprinting)
            {
                if (Input.GetKeyDown(zoomKey))
                {
                    isZoomed = true;
                }
                else if (Input.GetKeyUp(zoomKey))
                {
                    isZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if (isZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if (!isZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if (enableSprint)
        {
            if (isSprinting && !isCrouched)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                if (!unlimitedSprint)
                {
                    sprintRemaining -= Time.deltaTime;
                    sprintRemaining = Mathf.Clamp(sprintRemaining, 0f, sprintDuration);
                    if (sprintRemaining <= 0f)
                    {
                        isSprinting = false;
                    }
                }
            }
            else
            {
                sprintRemaining = Mathf.Clamp(sprintRemaining + 1 * Time.deltaTime, 0, sprintDuration);

                // Si la barra NO está llena, estamos en cooldown (aunque isSprintCooldown sea false, indicamos que estamos en recuperación)
                if (sprintRemaining < sprintDuration)
                {
                    isSprintCooldown = true;
                }
                else
                {
                    isSprintCooldown = false;
                }
            }

            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    sprintCooldown = sprintCooldownReset;
                    // Ya se terminó cooldown, si la barra está llena, se pasa a respiración normal
                    if (sprintRemaining >= sprintDuration)
                    {
                        isSprintCooldown = false;
                    }
                }
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }

            // Audio respiración según estado
            if (isSprinting)
            {
                if (respiracionAudioSource.clip != respiracionAceleradaSprint || !respiracionAudioSource.isPlaying)
                {
                    respiracionAudioSource.clip = respiracionAceleradaSprint;
                    respiracionAudioSource.loop = true;
                    respiracionAudioSource.volume = 1.8f;
                    respiracionAudioSource.Play();
                }
            }
            else if (isSprintCooldown)
            {
                if (respiracionAudioSource.clip != respiracionAceleradaCooldown || !respiracionAudioSource.isPlaying)
                {
                    respiracionAudioSource.clip = respiracionAceleradaCooldown;
                    respiracionAudioSource.loop = true;
                    respiracionAudioSource.volume = 1.8f;
                    respiracionAudioSource.Play();
                }
            }
            else
            {
                if (respiracionAudioSource.clip != respiracionCalmadaNormal || !respiracionAudioSource.isPlaying)
                {
                    respiracionAudioSource.clip = respiracionCalmadaNormal;
                    respiracionAudioSource.loop = true;
                    respiracionAudioSource.volume = 0.02f;
                    respiracionAudioSource.Play();
                }
            }
        }




        #endregion

        #region Jump

        // Gets input and calls jump method
        if (enableJump && Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
            isJumping = true;
        }

        #endregion

        #region Crouch

        if (enableCrouch)
        {
            if (!holdToCrouch)
            {
                if (Input.GetKeyDown(crouchKey))
                {
                    ToggleCrouch();
                }
            }
            else
            {
                if (Input.GetKeyDown(crouchKey)&& isSprinting==false)
                {
                    isCrouched = true;
                    ApplyCrouchState();
                }
                else if (Input.GetKeyUp(crouchKey))
                {
                    isCrouched = false;
                    ApplyCrouchState();
                }
            }
        }
        Vector3 targetPos = isCrouched ? crouchingCamPos : standingCamPos;
        joint.localPosition = Vector3.Lerp(joint.localPosition, targetPos, Time.deltaTime * crouchTransitionSpeed);
        #endregion

        CheckGround();

        if (enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // All movement calculations shile sprint is active
            if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && isCrouched == false)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when valocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    isSprinting = true;
                    if (hideBarWhenFull && !unlimitedSprint)
                    {
                        sprintBarCG.alpha += 5 * Time.deltaTime;
                    }
                }

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                isSprinting = false;

                if (hideBarWhenFull && sprintRemaining == sprintDuration)
                {
                    sprintBarCG.alpha -= 3 * Time.deltaTime;
                }

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        #endregion
    }

    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if (isCrouched && !holdToCrouch)
        {
            ApplyCrouchState();
        }
    }

    private void ToggleCrouch()
    {
        isCrouched = !isCrouched;
        ApplyCrouchState();
    }

    private void ApplyCrouchState()
    {
        isSprinting = false;
        walkSpeed = isCrouched ? crouchSpeed : normalSpeed;
    }

    private Vector3 currentBasePos;

    private void HeadBob()
    {
        // Determina la posición objetivo según si está agachado o no
        Vector3 targetPos = isCrouched ? crouchingCamPos : standingCamPos;

        // Interpola suavemente hacia esa posición con la variable que tú definiste
        currentBasePos = Vector3.Lerp(currentBasePos, targetPos, Time.deltaTime * crouchTransitionSpeed);

        if (isWalking)
        {
            // Aumenta el timer según el estado
            if (isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            else if (isCrouched)
            {
                timer += Time.deltaTime * (bobSpeed * 0.5f);
            }
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }

            // Movimiento bob
            Vector3 bobOffset = new Vector3(
                Mathf.Sin(timer) * bobAmount.x,
                Mathf.Sin(timer * 2) * bobAmount.y,
                Mathf.Sin(timer) * bobAmount.z
            );

            // Suma el head bob a la posición interpolada
            joint.localPosition = currentBasePos + bobOffset;
        }
        else
        {
            // Cuando no camina, regresa suavemente a la posición base (sin bob)
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, currentBasePos, Time.deltaTime * crouchTransitionSpeed);
        }
    }
    private void FlashlightBob()
    {
        if (isWalking)
        {
            float currentSpeed = flashlightBobSpeed;

            if (isSprinting)
            {
                currentSpeed += 2f;
            }
            else if (isCrouched)
            {
                currentSpeed *= 0.5f;
            }

            flashlightBobTimer += Time.deltaTime * currentSpeed;

            Vector3 bobOffset = new Vector3(
                Mathf.Sin(flashlightBobTimer) * flashlightBobAmount.x,
                Mathf.Cos(flashlightBobTimer * 2) * flashlightBobAmount.y,
                0
            );

            flashlightTransform.localPosition = flashlightOriginalPos + bobOffset;
        }
        else
        {
            flashlightBobTimer = 0f;
            flashlightTransform.localPosition = Vector3.Lerp(
                flashlightTransform.localPosition,
                flashlightOriginalPos,
                Time.deltaTime * flashlightBobSpeed
            );
        }
    }





}