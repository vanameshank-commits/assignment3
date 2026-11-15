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

    void Update()
    {
        if (isLockpicking)
        {
            // Allow exiting the lock via Escape or Tab
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                StopLockpicking();
            }
            return;
        }

        // Raycast from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                // Prompt or press 'E' to begin
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartLockpicking();
                }
            }
        }
    }

    private void StartLockpicking()
    {
        isLockpicking = true;

        // 1. Position the lock Canvas slightly in front of the camera
        lockpickingAssembly.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * canvasSpawnDistance);

        // 2. Face the Canvas toward the camera
        lockpickingAssembly.transform.rotation = playerCamera.transform.rotation;

        // 3. Enable the lock visuals
        lockpickingAssembly.SetActive(true);

        // 4. Freeze player movement script
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
}