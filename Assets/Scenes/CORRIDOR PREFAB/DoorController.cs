using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isExitDoor = false;
    public string nextSceneName = "Marc_TestScene";

    [Tooltip("This script will automatically find '01_low'. Only this part will rotate.")]
    public Transform rotatingDoorPanel;

    [Header("Movement Settings")]
    public float openAngle = 90f;
    public float smoothTime = 5f;
    public float interactDistance = 4f;

    [Tooltip("Standard Swivel is Y (0,1,0). If it falls downward, try X (1,0,0). If it falls backward, try Z (0,0,1).")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    [Tooltip("Adjust this if the door is facing the wrong way when closed.")]
    public float rotationOffset = 0f;

    [Header("UI Instruction")]
    public GameObject interactionPrompt;

    private bool isOpen = false;
    private bool playerInRange = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private Coroutine animationCoroutine;
    private Transform playerTransform;

    void Start()
    {
        // Search children to find the '01' object (the panel)
        if (rotatingDoorPanel == null)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                // We strictly look for the panel so the frame (03) doesn't move
                if (child.name.Contains("01") && child != this.transform)
                {
                    rotatingDoorPanel = child;
                    break;
                }
            }
        }

        // Fallback if naming convention fails
        if (rotatingDoorPanel == null) rotatingDoorPanel = this.transform;

        // Save the original rotation of the panel
        closedRotation = rotatingDoorPanel.localRotation;
        targetRotation = closedRotation;

        // Find the player
        GameObject player = GameObject.Find("Marc");
        if (player == null) player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= interactDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (interactionPrompt != null) interactionPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E)) ToggleDoor();
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (interactionPrompt != null) interactionPrompt.SetActive(false);
            }
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        float angle = isOpen ? openAngle : 0f;

        // We multiply our chosen axis by the angle to ensure it only swivels on one plane
        Vector3 rotationEuler = rotationAxis * (angle + rotationOffset);
        targetRotation = closedRotation * Quaternion.Euler(rotationEuler);

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateDoor());
    }

    IEnumerator AnimateDoor()
    {
        float t = 0;
        while (t < 1)
        {
            // Use smoothTime to control the speed of the swing
            t += Time.deltaTime * (smoothTime / 2);
            rotatingDoorPanel.localRotation = Quaternion.Slerp(rotatingDoorPanel.localRotation, targetRotation, t);
            yield return null;
        }
        rotatingDoorPanel.localRotation = targetRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isExitDoor && (other.CompareTag("Player") || other.name == "Marc"))
        {
            if (isOpen) SceneManager.LoadScene(nextSceneName);
        }
    }
}