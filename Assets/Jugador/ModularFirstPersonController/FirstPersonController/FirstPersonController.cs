using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public int golpesRestantes = 3;

    [Header("Audio")]
    public AudioSource playerAudioSourceRespiracionLoop;
    public AudioSource playerAudioSourceLocomocionLoop;
    public AudioSource playerAudioSourceSFX;
    public Enemigo1 EnemigoSonidos;
    public AudioClip sonidoDolor;
    public AudioClip gritoMuerte;
    public AudioClip caminarClip;
    public AudioClip correrClip;
    public AudioClip saltoClip;
    public AudioClip respiracionCalmadaNormal;
    public AudioClip respiracionAceleradaSprint;
    public AudioClip respiracionAceleradaCooldown;

    [Header("Cámara y Efectos")]
    public Transform playerCamera1;
    private Vector3 camaraPosInicial;
    public Image sangrePantalla;
    public GameObject pantallaGameOver;

    [Header("Game Over")]
    public float tiempoReinicio = 4f;

    [SerializeField] private Transform flashlightTransform; // Asigna esto en el Inspector
    [SerializeField] private GameObject linterna;
    [SerializeField] private Vector3 flashlightBobAmount = new Vector3(0.02f, 0.02f, 0f);
    [SerializeField] private float flashlightBobSpeed = 6f;

    private Vector3 flashlightOriginalPos;
    private float flashlightBobTimer = 0f;

    private Rigidbody rb;
    
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
    private bool isRecovering = false;
    public bool estamuerto;
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

        cameraCanMove = true;
        Time.timeScale = 1f;

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
        if (pantallaGameOver != null)
            pantallaGameOver.SetActive(false);

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerAudioSourceRespiracionLoop.PlayOneShot(sonidoDolor);
        }

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
            if (isSprinting && !isCrouched && !unlimitedSprint)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                sprintRemaining -= Time.deltaTime;
                sprintRemaining = Mathf.Clamp(sprintRemaining, 0f, sprintDuration);

                if (sprintRemaining <= 0f)
                {
                    isSprinting = false;
                    isRecovering = true; // Inicia recuperación obligatoria
                }
            }
            else
            {
                // Recuperar sprint (solo si no está en uso)
                if (sprintRemaining < sprintDuration)
                {
                    sprintRemaining = Mathf.Clamp(sprintRemaining + 1 * Time.deltaTime, 0, sprintDuration);
                }

                // Finaliza la recuperación cuando la barra está llena
                if (isRecovering && sprintRemaining >= sprintDuration)
                {
                    isRecovering = false;
                }
            }

            // Control del cooldown visual
            if (sprintRemaining < sprintDuration)
            {
                isSprintCooldown = true;
            }
            else if (!isRecovering)
            {
                isSprintCooldown = false;
            }

            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    sprintCooldown = sprintCooldownReset;
                }
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            // Actualizar barra de sprint visual
            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }

            // Reproducir sonido de respiración según el estado
            if (isSprinting)
            {
                PlayLoopRespiracion(respiracionAceleradaSprint, 1.8f);
            }
            else if (isRecovering || isSprintCooldown)
            {
                PlayLoopRespiracion(respiracionAceleradaCooldown, 1.8f);
            }
            else if(!estamuerto)
            {
                PlayLoopRespiracion(respiracionCalmadaNormal, 0.02f);
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
            isWalking = (targetVelocity.x != 0 || targetVelocity.z != 0) && isGrounded;
            HandleLocomotionAudio(targetVelocity);

            // All movement calculations while sprint is active
            if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && isCrouched == false && isWalking && !isRecovering)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when velocity change != 0
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
            // All movement calculations while walking or recovering
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

    public void RecibirDanio()
    {
        if (golpesRestantes <= 0) return;

        golpesRestantes--;

        // Sonido de dolor
        playerAudioSourceSFX.PlayOneShot(sonidoDolor);

        // Efecto de sangre en pantalla
        if (sangrePantalla != null)
        {
            float alpha = 1f - (golpesRestantes / 3f); // más rojo con más daño
            sangrePantalla.color = new Color(1, 0, 0, alpha * 0.6f); // rojo semitransparente
        }

        // Iniciar sacudida intensa
        if (playerCamera1 != null)
            StartCoroutine(SacudirCamara(1f, 0.5f));
            StartCoroutine(SacudirLinterna(1f, 0.5f));

        // Game Over
        if (golpesRestantes <= 0)
        {
            playerAudioSourceSFX.PlayOneShot(gritoMuerte);
            Destroy(linterna);
            StartCoroutine(AnimacionMuerte());
        }
    }

    private IEnumerator SacudirCamara(float duracion, float intensidad)
    {
        Vector3 posicionInicial = playerCamera1.localPosition;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            float desplazamientoX = Mathf.Sin(tiempo * 10f) * intensidad * (1f - (tiempo / duracion));
            float desplazamientoY = Mathf.Cos(tiempo * 8f) * intensidad * (1f - (tiempo / duracion));

            playerCamera1.localPosition = Vector3.Lerp(playerCamera1.localPosition,
                posicionInicial + new Vector3(desplazamientoX, desplazamientoY, 0f),
                0.4f);

            tiempo += Time.unscaledDeltaTime;
            yield return null;
        }

        playerCamera1.localPosition = posicionInicial;
    }
    public IEnumerator SacudirLinterna(float duracion, float intensidad)
    {
        float tiempo = 0f;
        Vector3 originalPos = flashlightOriginalPos;

        while (tiempo < duracion)
        {
            float offsetX = Mathf.Sin(tiempo * 8f) * intensidad * (1f - (tiempo / duracion));
            float offsetY = Mathf.Cos(tiempo * 6f) * intensidad * (1f - (tiempo / duracion));

            flashlightTransform.localPosition = Vector3.Lerp(
                flashlightTransform.localPosition,
                originalPos + new Vector3(offsetX, offsetY, 0f),
                0.4f
            );

            tiempo += Time.unscaledDeltaTime;
            yield return null;
        }

        flashlightTransform.localPosition = originalPos;
    }
    private IEnumerator AnimacionMuerte()
    {
        float duracion = 1.5f;
        float tiempo = 0f;

        Quaternion rotacionInicial = playerCamera1.localRotation;
        Quaternion rotacionFinal = Quaternion.Euler(270f, playerCamera1.localEulerAngles.y, 0f); // Mira hacia arriba

        while (tiempo < duracion)
        {
            playerCamera1.localRotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        playerCamera1.localRotation = rotacionFinal;
        cameraCanMove = false;
        yield return new WaitForSeconds(3f);

        StartCoroutine(MostrarGameOverPanel());
}


    private IEnumerator MostrarGameOverPanel()
    {
        estamuerto = true;
        float duracion = 1f;
        float tiempo = 0f;
        pantallaGameOver.SetActive(true);
        CanvasGroup cg = pantallaGameOver.GetComponent<CanvasGroup>();
        StopLoopingSoundRespiracion();
        EnemigoSonidos.StopLoopingSound();
        if (cg == null)
        {
            cg = pantallaGameOver.AddComponent<CanvasGroup>();
        }

        cg.alpha = 0f;

        while (tiempo < duracion)
        {
            cg.alpha = Mathf.Lerp(0f, 1f, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        cameraCanMove = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StopLoopingSoundRespiracion()
    {
        if (playerAudioSourceRespiracionLoop != null)
        {
            playerAudioSourceRespiracionLoop.Stop();
            playerAudioSourceRespiracionLoop.clip = null;
            playerAudioSourceRespiracionLoop.loop = false;
        }
    }
    void PlayLoopRespiracion(AudioClip clip, float volumen = 1f)
    {
        if (clip == null || playerAudioSourceRespiracionLoop == null) return;

        if (playerAudioSourceRespiracionLoop.clip != clip)
        {
            playerAudioSourceRespiracionLoop.Stop();
            playerAudioSourceRespiracionLoop.clip = clip;
            playerAudioSourceRespiracionLoop.volume = volumen;
            playerAudioSourceRespiracionLoop.loop = true;
            playerAudioSourceRespiracionLoop.Play();
        }
    }
    public void StopLoopingSoundLocomocion()
    {
        if (locomocionFadeCoroutine != null)
        {
            StopCoroutine(locomocionFadeCoroutine);
            locomocionFadeCoroutine = null;
        }

        if (playerAudioSourceLocomocionLoop != null)
        {
            playerAudioSourceLocomocionLoop.Stop();
            playerAudioSourceLocomocionLoop.clip = null;
            playerAudioSourceLocomocionLoop.loop = false;
            playerAudioSourceLocomocionLoop.volume = 0f;
        }
    }

    private Coroutine locomocionFadeCoroutine;

    void PlayLoopLocomocion(AudioClip clip, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (clip == null || playerAudioSourceLocomocionLoop == null) return;

        if (playerAudioSourceLocomocionLoop.clip == clip && playerAudioSourceLocomocionLoop.isPlaying)
            return;

        // Si ya hay una corrutina corriendo, detenerla
        if (locomocionFadeCoroutine != null)
            StopCoroutine(locomocionFadeCoroutine);

        locomocionFadeCoroutine = StartCoroutine(FadeToNewLocomocionClip(clip, targetVolume, fadeDuration));
    }


    void PlaySFX(AudioClip clip, float volumen = 1f)
    {
        if (clip == null || playerAudioSourceSFX == null) return;

        playerAudioSourceSFX.PlayOneShot(clip, volumen);
    }

    private void HandleLocomotionAudio(Vector3 targetVelocity)
    {
        if (!isGrounded || targetVelocity.magnitude == 0f)
        {
            StopLoopingSoundLocomocion();
            return;
        }

        if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && !isCrouched)
        {
            PlayLoopLocomocion(correrClip, 1f);
        }
        else if (targetVelocity.magnitude > 0.1f)
        {
            PlayLoopLocomocion(caminarClip, 0.8f);
        }
    }



    private IEnumerator FadeToNewLocomocionClip(AudioClip newClip, float targetVolume, float fadeDuration=0.2f)
    {
        AudioSource source = playerAudioSourceLocomocionLoop;

        float startVolume = source.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        source.Stop();
        source.clip = newClip;
        source.loop = true;
        source.volume = 0f;
        source.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;
    }





}