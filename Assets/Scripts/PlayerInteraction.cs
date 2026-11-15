using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public float canvasSpawnDistance = 0.8f; // How far in front of the camera the lock appears

    [Header("References")]
    public GameObject lockpickingAssembly;
    public FirstPersonController playerController;
    public Camera playerCamera;

    private bool isLockpicking = false;
    private DoorController currentDoor;

    void Update()
    {
        // 1. If we are currently lockpicking, listen for the exit buttons
        if (isLockpicking)
        {
            // Allow exiting the lock via Escape or Tab
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                StopLockpicking();
            }
            return;
        }

        // 2. Shoot raycast from the center of the screen to detect doors
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                // Prompt or press 'E' to begin
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Remember which door we are looking at
                    currentDoor = hit.collider.GetComponent<DoorController>();
                    StartLockpicking();
                }
            }
        }
    }

    private void StartLockpicking()
    {
        isLockpicking = true;

        // Position the lock Canvas slightly in front of the camera
        lockpickingAssembly.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * canvasSpawnDistance);

        // Face the Canvas toward the camera
        lockpickingAssembly.transform.rotation = playerCamera.transform.rotation;

        // Enable the lock visuals and freeze player movement
        lockpickingAssembly.SetActive(true);
        playerController.enabled = false;

        // CRITICAL FIX: Unlock and show cursor so mouse-look moves the pick instead of the camera
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StopLockpicking()
    {
        isLockpicking = false;
        lockpickingAssembly.SetActive(false);

        // Re-enable player movement
        playerController.enabled = true;

        // Re-lock and hide cursor for FPS gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCurrentDoor()
    {
        if (currentDoor != null)
        {
            currentDoor.OpenDoor();
        }
        StopLockpicking(); // This automatically hides the canvas and gives player control back
    }
}