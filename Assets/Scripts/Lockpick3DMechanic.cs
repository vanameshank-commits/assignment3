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

    private float currentPickAngle = 0f;
    private float cylinderRotation = 0f;
    private bool isApplyingTorque = false;
    private bool isUnlocked = false;

    void Start()
    {
        // Randomize the sweet spot for this lock
        sweetSpotAngle = Random.Range(-maxAngle, maxAngle);
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

            // Apply the rotation to the tension tool/cylinder (usually around the Z axis in 2D/3D hybrids)
            tensionTool.localEulerAngles = new Vector3(0, 0, cylinderRotation);

            // Win Condition
            if (cylinderRotation >= 89.9f && distanceFromSweetSpot <= sweetSpotTolerance)
            {
                isUnlocked = true;
                Debug.Log("Lock Unlocked!");
            }
            // Jammed Condition
            else if (cylinderRotation >= maxAllowedRotation)
            {
                ShakeRakeTool3D();
            }
        }
        else
        {
            isApplyingTorque = false;

            // Snap back when released
            if (cylinderRotation > 0)
            {
                cylinderRotation -= cylinderTurnSpeed * Time.deltaTime * 4f;
                cylinderRotation = Mathf.Clamp(cylinderRotation, 0, 90f);
                tensionTool.localEulerAngles = new Vector3(0, 0, cylinderRotation);
            }
        }
    }

    private void ShakeRakeTool3D()
    {
        // Visual jitter to show the tool is jammed and taking damage
        float jitter = Random.Range(-1.5f, 1.5f);
        rakePivot.localEulerAngles = new Vector3(0, 0, currentPickAngle + jitter);
    }
}