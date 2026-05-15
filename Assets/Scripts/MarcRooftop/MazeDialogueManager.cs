using UnityEngine;
using System.Collections;

public class MazeDialogueManager : MonoBehaviour
{
    public static MazeDialogueManager Instance;

    public SimpleDialogue dialogueSystem;
    public GameObject dialogueChoicePanel;

    [Header("Movement Control")]
    public MonoBehaviour playerMovementScript;

    [Header("UI Positioning Fix")]
    [Tooltip("The coordinates you provided for the working UI position.")]
    public Vector2 workingAnchoredPosition = new Vector2(-236.36f, -318.2f);

    private int doorsOpened = 0;

    void Awake()
    {
        Instance = this;
        Debug.Log("<color=white>MazeDialogueManager:</color> Script Awake and Running.");
    }

    void Start()
    {
        if (dialogueSystem == null) Debug.LogError("MazeDialogueManager: DialogueSystem is NOT assigned!");
        if (playerMovementScript == null) SetPlayerMovement(true);
    }

    void Update()
    {
        // DEBUG TRIGGER: Press T to force the UI to appear
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerConversation("System", "Testing UI visibility... UI forced to Overlay and snapped to your working coordinates.");
        }
    }

    public void OnDoorOpened()
    {
        if (dialogueSystem != null && dialogueSystem.vnUI != null && dialogueSystem.vnUI.activeInHierarchy) return;

        doorsOpened++;
        Debug.Log("<color=green>Interaction Successful:</color> Door Opened: " + doorsOpened);

        if (doorsOpened == 1)
        {
            TriggerConversation("Darlene", "Bukas na yung pinto... pero parang mas dumidilim sa loob.");
        }
        else
        {
            string[] clues = { "May narinig ba kayo?", "Marc, 'wag ka masyadong malayo.", "Raven, look at the walls." };
            TriggerConversation("Darlene", clues[Random.Range(0, clues.Length)]);
        }
    }

    void TriggerConversation(string speaker, string line)
    {
        if (dialogueSystem == null) return;

        SetPlayerMovement(false); // Stop player

        // --- THE "FORCE VISIBLE" FIX ---
        if (dialogueSystem.vnUI != null)
        {
            // 1. Force the Canvas to draw on top of everything (Overlay)
            Canvas canvas = dialogueSystem.vnUI.GetComponentInParent<Canvas>();
            if (canvas != null) canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            dialogueSystem.vnUI.SetActive(true);

            // 2. Force Transparency to 100%
            CanvasGroup cg = dialogueSystem.vnUI.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;

            // 3. Snap TextBox to the specific working coordinates you gave me
            if (dialogueSystem.textBox != null)
            {
                dialogueSystem.textBox.SetActive(true);
                RectTransform rect = dialogueSystem.textBox.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = workingAnchoredPosition;
                    Debug.Log($"UI Snapped to: {workingAnchoredPosition}");
                }
                // Bring to front
                dialogueSystem.textBox.transform.SetAsLastSibling();
            }

            dialogueSystem.vnUI.transform.SetAsLastSibling();
        }

        dialogueSystem.ShowDialogue(speaker, line);
        StartCoroutine(WaitToResume());
    }

    IEnumerator WaitToResume()
    {
        yield return new WaitForSeconds(0.5f);
        if (dialogueSystem != null && dialogueSystem.vnUI != null)
        {
            yield return new WaitUntil(() => dialogueSystem.vnUI.activeInHierarchy == false);
            SetPlayerMovement(true); // Resume player
        }
    }
    
    void SetPlayerMovement(bool canMove)
    {
        if (playerMovementScript != null) playerMovementScript.enabled = canMove;
        else
        {
            GameObject p = GameObject.Find("Marc");
            if (p != null)
            {
                var moveComps = p.GetComponents<MonoBehaviour>();
                foreach (var comp in moveComps)
                {
                    if (comp.GetType().Name.ToLower().Contains("movement"))
                    {
                        comp.enabled = canMove;
                        playerMovementScript = comp;
                        break;
                    }
                }
            }
        }
    }
}