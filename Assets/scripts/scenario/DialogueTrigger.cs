using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public Dialogue nextDialogue;

    public int timeCost; // ⏳ Time cost (0 = no deduction)
}


[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
    public List<DialogueChoice> choices = new List<DialogueChoice>(); // Choices for branching dialogue
}


[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject exclamationMark; // Reference to the exclamation mark UI element

    public bool sceneswitch = false;


    private bool playerInRange = false;
    private bool isInteracting = false;

    private void Start()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false); // Hide exclamation mark by default
        }
    }

    private void Update()
    {
        if (playerInRange && !isInteracting)
        {
            if (exclamationMark != null)
            {
                exclamationMark.SetActive(true); // Show exclamation mark when player is near
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false); // Hide exclamation mark when starting dialogue
        }

        isInteracting = true;

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void EndDialogue()
    {
        isInteracting = false;

        if (sceneswitch == true)
        {
            // Optional: Add a fade or delay before switching
            UnityEngine.SceneManagement.SceneManager.LoadScene("Dream"); // 👈 Change to your actual scene name
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            isInteracting = false;
            if (exclamationMark != null)
            {
                exclamationMark.SetActive(false); // Hide exclamation mark when leaving range
            }
        }
    }
}
