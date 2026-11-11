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
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                StopLockpicking();
            }
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
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

        // 1. Teleport the Canvas to be slightly in front of the camera
        lockpickingAssembly.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * canvasSpawnDistance);

        // 2. Rotate the Canvas so it perfectly faces the camera
        lockpickingAssembly.transform.rotation = playerCamera.transform.rotation;

        // 3. Turn on the lock visuals
        lockpickingAssembly.SetActive(true);

        // 4. Freeze the player
        playerController.enabled = false;
    }

    public void StopLockpicking()
    {
        isLockpicking = false;
        lockpickingAssembly.SetActive(false);
        playerController.enabled = true;
    }
}