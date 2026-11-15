using UnityEngine;

public class Lockpick3DMechanic : MonoBehaviour
{
    [Header("3D Transforms")]
    public Transform tensionTool; // The wrench/cylinder that rotates under tension
    public Transform rakePivot;   // The pivot point for your rake tool

    [Header("Lock Settings")]
    public float maxAngle = 90f;
    public float sweetSpotTolerance = 12f;
    private float sweetSpotAngle;

    [Header("Input & Feel")]
    public float pickTurnSpeed = 250f;
    public float cylinderTurnSpeed = 120f;

    [Header("Audio Setup")]
    public AudioSource audioSource;          // Dedicated source for scraping/continuous sounds
    public AudioSource oneShotAudioSource;   // Secondary source for clicks, win, break sounds
    public AudioClip scrapeClip;             // Metal scraping loop when turning
    public AudioClip clickClip;              // Proximity click when close to sweet spot
    public AudioClip successClip;            // Unlocked chime
    public AudioClip breakClip;              // Pick snap sound (optional future use)

    private float currentPickAngle = 0f;
    private float cylinderRotation = 0f;
    private bool isApplyingTorque = false;
    private bool isUnlocked = false;
    private float clickTimer = 0f;

    void Start()
    {
        // Randomize the sweet spot for this lock instance
        sweetSpotAngle = Random.Range(-maxAngle + sweetSpotTolerance, maxAngle - sweetSpotTolerance);

        if (audioSource != null)
        {
            audioSource.clip = scrapeClip;
            audioSource.loop = true;
        }
    }

    void Update()
    {
        if (isUnlocked) return;

        HandleRakeMovement();
        HandleTensionTorque();
    }

    private void HandleRakeMovement()
    {
        // Rake only swings if the player isn't applying tension
        if (!isApplyingTorque)
        {
            float mouseInput = Input.GetAxis("Mouse X");
            currentPickAngle -= mouseInput * pickTurnSpeed * Time.deltaTime;

            // Keep the rake within the keyhole limits
            currentPickAngle = Mathf.Clamp(currentPickAngle, -maxAngle, maxAngle);

            // Swing the rake around its pivot
            rakePivot.localEulerAngles = new Vector3(0, 0, currentPickAngle);
        }
    }

    private void HandleTensionTorque()
    {
        // Apply torque with Spacebar
        if (Input.GetKey(KeyCode.Space))
        {
            isApplyingTorque = true;

            float distanceFromSweetSpot = Mathf.Abs(currentPickAngle - sweetSpotAngle);
            float maxAllowedRotation = 90f;

            if (distanceFromSweetSpot > sweetSpotTolerance)
            {
                float distanceRatio = 1f - (distanceFromSweetSpot / maxAngle);
                maxAllowedRotation = 90f * Mathf.Clamp01(distanceRatio);
            }

            // Rotate the tension tool
            cylinderRotation += cylinderTurnSpeed * Time.deltaTime;
            cylinderRotation = Mathf.Clamp(cylinderRotation, 0, maxAllowedRotation);

            // Apply rotation to the tension tool/cylinder
            tensionTool.localEulerAngles = new Vector3(0, 0, cylinderRotation);

            // Handle Scraping Audio Loop
            if (audioSource != null && !audioSource.isPlaying && scrapeClip != null)
            {
                audioSource.Play();
            }

            // Play procedural clicks based on how close you are to the sweet spot
            PlayProximityClicks(distanceFromSweetSpot);

            // Win Condition
            if (cylinderRotation >= 89.9f && distanceFromSweetSpot <= sweetSpotTolerance)
            {
                UnlockSuccess();
            }
            // Jammed Condition
            else if (cylinderRotation >= maxAllowedRotation)
            {
                ShakeRakeTool3D();
            }
        }
        else
        {
            StopTorqueInput();
        }
    }

    private void StopTorqueInput()
    {
        isApplyingTorque = false;

        // Stop scraping audio when releasing spacebar
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Snap cylinder back when spacebar is released
        if (cylinderRotation > 0)
        {
            cylinderRotation -= cylinderTurnSpeed * Time.deltaTime * 4f;
            cylinderRotation = Mathf.Clamp(cylinderRotation, 0, 90f);
            tensionTool.localEulerAngles = new Vector3(0, 0, cylinderRotation);
        }
    }

    private void PlayProximityClicks(float distance)
    {
        // The closer the pick is, the faster it clicks (like a combination lock / Skyrim tension)
        // If distance is huge, click interval is long; if close, click interval is short.
        float clickInterval = Mathf.Lerp(0.05f, 0.5f, Mathf.Clamp01(distance / maxAngle));

        clickTimer += Time.deltaTime;
        if (clickTimer >= clickInterval)
        {
            clickTimer = 0f;
            if (oneShotAudioSource != null && clickClip != null)
            {
                // Pitch increases slightly the closer you are to the sweet spot
                float pitch = Mathf.Lerp(1.5f, 0.8f, Mathf.Clamp01(distance / sweetSpotTolerance));
                oneShotAudioSource.pitch = pitch;
                oneShotAudioSource.PlayOneShot(clickClip, 0.6f);
            }
        }
    }

    private void ShakeRakeTool3D()
    {
        // Visual jitter to show the tool is jammed against the wall
        float jitter = Random.Range(-1.5f, 1.5f);
        rakePivot.localEulerAngles = new Vector3(0, 0, currentPickAngle + jitter);

        // Optional: Play a metallic clank or tension strain sound here
    }

    private void UnlockSuccess()
    {
        isUnlocked = true;
        Debug.Log("Lock Unlocked!");

        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
        if (oneShotAudioSource != null && successClip != null)
        {
            oneShotAudioSource.pitch = 1f;
            oneShotAudioSource.PlayOneShot(successClip);
        }

        // TODO: Trigger door opening animation / call StopLockpicking() after a short delay
    }
}