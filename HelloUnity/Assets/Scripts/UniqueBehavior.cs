using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using BTAI; // Import Behavior Tree AI namespace

// Main script for NPC dialogue and behavior using Behavior Tree structure
public class NPCDialogueBehaviorTree : MonoBehaviour
{
    // UI elements for dialogue, quest hints, and interaction prompt
    public Canvas dialogueCanvas;          // Canvas that displays the dialogue text
    public TextMeshProUGUI dialogueText;   // TextMeshPro component to display the dialogue text
    public GameObject questHintPanel;      // Panel to show quest-related hints
    public Canvas interactionCanvas;       // Canvas showing prompt to interact with NPC (e.g., "Press E to Interact")
    public Transform player;               // Reference to the player object
    public float interactionRange = 3f;    // The range within which the player can interact with the NPC

    // Internal flags to track state of interactions
    private bool hasWelcomedPlayer = false; // Tracks whether the welcome dialogue has been shown
    private bool questStarted = false;     // Tracks whether the quest dialogue has been shown
    private Root m_btRoot = BT.Root();     // Root of the behavior tree
    private bool isInteractionPromptVisible = false; // Tracks whether the interaction prompt is visible

    void Start()
    {
        // Ensure UI elements are initially hidden
        if (dialogueCanvas != null) dialogueCanvas.enabled = false;
        if (interactionCanvas != null) interactionCanvas.enabled = false;
        if (questHintPanel != null) questHintPanel.SetActive(false);

        // Build the behavior tree with branches and actions
        m_btRoot.OpenBranch(
            BT.If(PlayerInRange).OpenBranch(
                BT.Call(ShowInteractionPrompt), // Show the interaction prompt if the player is in range
                BT.If(PlayerInteracted).OpenBranch(
                    BT.Call(HandleDialogue) // Handle dialogue when the player interacts with the NPC
                )
            ),
            BT.If(PlayerOutOfRange).OpenBranch(
                BT.Call(HideUI) // Hide UI if the player is out of range
            )
        );
    }

    void Update()
    {
        m_btRoot.Tick(); // Tick the behavior tree each frame
    }

    // Behavior tree conditions

    // Check if the player is within interaction range
    bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }

    // Check if the player is outside the interaction range
    bool PlayerOutOfRange()
    {
        return !PlayerInRange();
    }

    // Check if the player has interacted with the NPC (e.g., pressed the "E" or "F" key)
    bool PlayerInteracted()
    {
        return Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.F) && hasWelcomedPlayer && !questStarted);
    }

    // Behavior tree actions

    // Show the interaction prompt (e.g., "Press E to Interact")
    void ShowInteractionPrompt()
    {
        if (!isInteractionPromptVisible && interactionCanvas != null)
        {
            StartCoroutine(FadeCanvas(interactionCanvas, true)); // Smooth fade-in
            isInteractionPromptVisible = true;
        }
    }

    // Handle dialogue actions based on player interaction
    void HandleDialogue()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If dialogue is already showing, hide it and reset
            if (dialogueCanvas.enabled)
            {
                HideDialogue();
                ResetDialogue();
            }
            else
            {
                // Show the initial welcome dialogue
                ShowDialogue("Welcome to the game");
                hasWelcomedPlayer = true;

                // Show quest hint if quest hasn't started yet
                if (questHintPanel != null && !questStarted)
                {
                    questHintPanel.SetActive(true);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && hasWelcomedPlayer && !questStarted)
        {
            // Start the quest dialogue if the player has interacted after the welcome dialogue
            ShowQuestDialogue("First quest!\nGet 10 Coins: Kill slimes to get coins!");
            questStarted = true;

            // Hide the quest hint panel once quest is started
            if (questHintPanel != null)
            {
                questHintPanel.SetActive(false);
            }
        }
    }

    // Hide all UI elements (interaction prompt, dialogue, and quest hints)
    void HideUI()
    {
        if (isInteractionPromptVisible && interactionCanvas != null)
        {
            StartCoroutine(FadeCanvas(interactionCanvas, false)); // Smooth fade-out
            isInteractionPromptVisible = false;
        }

        if (dialogueCanvas != null && dialogueCanvas.enabled)
        {
            HideDialogue();
        }

        if (questHintPanel != null)
        {
            questHintPanel.SetActive(false);
        }

        ResetDialogue(); // Reset dialogue state
    }

    // Dialogue functions

    // Show the dialogue with a given message
    void ShowDialogue(string message)
    {
        if (dialogueCanvas != null && dialogueText != null)
        {
            dialogueText.text = message;
            StartCoroutine(FadeCanvas(dialogueCanvas, true)); // Smooth fade-in
        }
        else
        {
            Debug.LogError("DialogueCanvas or DialogueText is not assigned!"); // Handle missing UI references
        }
    }

    // Show the quest dialogue with a given message
    void ShowQuestDialogue(string message)
    {
        if (dialogueCanvas != null && dialogueText != null)
        {
            dialogueText.text = message;
            StartCoroutine(FadeCanvas(dialogueCanvas, true)); // Smooth fade-in
        }
        else
        {
            Debug.LogError("DialogueCanvas or DialogueText is not assigned!"); // Handle missing UI references
        }
    }

    // Hide the dialogue canvas (smooth fade-out)
    void HideDialogue()
    {
        if (dialogueCanvas != null)
        {
            StartCoroutine(FadeCanvas(dialogueCanvas, false)); // Smooth fade-out
        }
    }

    // Reset the dialogue-related flags
    void ResetDialogue()
    {
        hasWelcomedPlayer = false;
        questStarted = false;
    }

    // Smoothly fade in or out a given canvas
    IEnumerator FadeCanvas(Canvas canvas, bool fadeIn)
    {
        // Check for an existing CanvasGroup to control alpha (transparency)
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>(); // Add a CanvasGroup if it doesn't exist
        }

        // Set the initial and target alpha values based on fade direction
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float duration = 0.5f; // Duration of the fade effect

        // Perform the fade effect
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration); // Interpolate alpha over time
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure the final alpha is set

        // Disable canvas after fade-out, or enable it after fade-in
        if (!fadeIn)
        {
            canvas.enabled = false;
        }
        else
        {
            canvas.enabled = true;
        }
    }
}
