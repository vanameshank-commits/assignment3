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

<<<<<<< Updated upstream
        // Raycast from the center of the screen
=======
        // 2. Shoot raycast to detect doors
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        // 1. Position the lock Canvas slightly in front of the camera
        lockpickingAssembly.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * canvasSpawnDistance);

        // 2. Face the Canvas toward the camera
        lockpickingAssembly.transform.rotation = playerCamera.transform.rotation;

        // 3. Enable the lock visuals
        lockpickingAssembly.SetActive(true);

        // 4. Freeze player movement script
=======
        // Teleport the Canvas to be slightly in front of the camera
        lockpickingAssembly.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * canvasSpawnDistance);

        // Rotate the Canvas so it perfectly faces the camera
        lockpickingAssembly.transform.rotation = playerCamera.transform.rotation;

        // Turn on the lock visuals and freeze the player
        lockpickingAssembly.SetActive(true);
>>>>>>> Stashed changes
        playerController.enabled = false;

        // 5. CRITICAL FIX: Unlock and show cursor so mouse-look moves the pick instead of the camera
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