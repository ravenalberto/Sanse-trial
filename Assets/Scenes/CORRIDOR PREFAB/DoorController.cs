using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Transition Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.0f;

    [Header("Door Settings")]
    public bool isExitDoor = false;
    public Transform rotatingDoorPanel; // Should be the '01_low' child

    [Header("Movement Settings")]
    public float openAngle = 90f;
    public float smoothTime = 3f;
    public float interactDistance = 5f;
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    [Header("UI Instruction")]
    public GameObject interactionPrompt;

    private bool isOpen = false;
    private bool isTransitioning = false;
    private bool playerInRange = false;
    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private Coroutine animationCoroutine;
    private Transform playerTransform;

    void Start()
    {
        // Automatically find the rotating panel if not assigned
        if (rotatingDoorPanel == null)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("01") && child != this.transform)
                {
                    rotatingDoorPanel = child;
                    break;
                }
            }
        }

        if (rotatingDoorPanel == null) rotatingDoorPanel = this.transform;

        closedRotation = rotatingDoorPanel.localRotation;
        targetRotation = closedRotation;

        // Find the player object
        GameObject player = GameObject.Find("Marc");
        if (player == null) player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (interactionPrompt != null) interactionPrompt.SetActive(false);

        // Setup Collider for Exit Door functionality
        if (isExitDoor && rotatingDoorPanel != null)
        {
            BoxCollider col = rotatingDoorPanel.GetComponent<BoxCollider>();
            if (col == null) col = rotatingDoorPanel.gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;

            // Adding the TriggerForwarder to the child panel
            if (rotatingDoorPanel.gameObject.GetComponent<TriggerForwarder>() == null)
            {
                var forwarder = rotatingDoorPanel.gameObject.AddComponent<TriggerForwarder>();
                forwarder.parentController = this;
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || isTransitioning) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= interactDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (interactionPrompt != null) interactionPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("<color=green>Interaction:</color> E Pressed!");
                ToggleDoor();
            }
        }
        else if (playerInRange)
        {
            playerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }

    public void ToggleDoor()
    {
        if (isExitDoor && !isOpen)
        {
            isOpen = true;
            // Notify the Dialogue System
            MazeDialogueManager manager = Object.FindAnyObjectByType<MazeDialogueManager>();
            if (manager != null) manager.OnDoorOpened();

            // Start the fade and scene load
            StartCoroutine(FadeAndLoadScene("Scene04"));
            return;
        }

        // Standard rotation toggle
        isOpen = !isOpen;
        float angle = isOpen ? openAngle : 0f;
        targetRotation = closedRotation * Quaternion.Euler(rotationAxis * angle);

        // Notify the Dialogue System
        MazeDialogueManager diagManager = Object.FindAnyObjectByType<MazeDialogueManager>();
        if (diagManager != null) diagManager.OnDoorOpened();

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateDoor());
    }

    IEnumerator AnimateDoor()
    {
        while (Quaternion.Angle(rotatingDoorPanel.localRotation, targetRotation) > 0.1f)
        {
            rotatingDoorPanel.localRotation = Quaternion.Slerp(
                rotatingDoorPanel.localRotation,
                targetRotation,
                Time.deltaTime * smoothTime
            );
            yield return null;
        }
        rotatingDoorPanel.localRotation = targetRotation;
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        isTransitioning = true;
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("Fade Canvas Group is missing! Assign it in Inspector.");
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadeCanvasGroup.blocksRaycasts = true;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene '" + sceneName + "' not in Build Settings!");
        }
    }

    public void HandleTrigger(Collider other)
    {
        if (isExitDoor && isOpen && !isTransitioning)
        {
            if (other.CompareTag("Player") || other.name == "Marc")
            {
                StartCoroutine(FadeAndLoadScene("Scene04"));
            }
        }
    }
}

// This class must exist outside the DoorController class but in the same file
public class TriggerForwarder : MonoBehaviour
{
    public DoorController parentController;
    private void OnTriggerEnter(Collider other)
    {
        if (parentController != null) parentController.HandleTrigger(other);
    }
}