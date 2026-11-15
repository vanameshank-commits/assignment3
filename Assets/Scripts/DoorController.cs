using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool isOpen = false;
    private Quaternion targetRotation;

    public float smoothSpeed = 3f;
    public float openAngle = 90f;

    [Header("Axis Fix")]
    [Tooltip("If the door flips the wrong way, try setting X to 1 and Y to 0, or Z to 1.")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0); // Default is Y axis

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }

    public void OpenDoor()
    {
        isOpen = true;

        // Multiply your chosen axis by the 90 degree angle
        Vector3 finalRotation = rotationAxis.normalized * openAngle;
        targetRotation = transform.rotation * Quaternion.Euler(finalRotation);

        gameObject.tag = "Untagged";
    }
}