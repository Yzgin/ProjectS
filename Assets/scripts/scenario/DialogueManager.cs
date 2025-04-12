using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public GameObject DialoguePanel;  // Reference to the interaction UI Panel
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    private DialogueLine currentLine;

    public MonoBehaviour playerMovementScript; // Player movement script to disable movement when talking
    public GameObject choicePanel; // Panel containing choices
    public bool isDialogueActive = false;
    public float typingSpeed = 0.05f; // Adjust for speed

    private bool isTyping = false; // To track if typing is in progress
    private bool canPressSpace = true; // Prevent spamming

    public GameObject fadePanel; // Reference to the fade panel
    public float fadeDuration = 1.0f; // Adjust for desired fade speed

    private IEnumerator FadeEffect()
    {
        yield return StartCoroutine(Fade(1)); // Fade out

        // Deduct time after fading out
        TimeSystem.Instance.DeductTime(2);

        yield return StartCoroutine(Fade(0)); // Fade in
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Image panelImage = fadePanel.GetComponent<Image>(); // Get the Image component

        if (panelImage == null)
        {
            Debug.LogError("FadePanel does not have an Image component!");
            yield break;
        }

        Color fadeColor = panelImage.color; // Get the current color
        float startAlpha = fadeColor.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeColor.a = newAlpha; // Update only the alpha
            panelImage.color = fadeColor; // Assign the updated color back
            yield return null;
        }

        fadeColor.a = targetAlpha;
        panelImage.color = fadeColor; // Ensure final color is set
    }



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        lines = new Queue<DialogueLine>();
    }

    private void Start()
    {
        DialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
    }

    private void Update()
    {

        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpacebarCooldown());
            if (isTyping)
            {
                // Instantly show full text if player presses Space while typing
                StopAllCoroutines();
                dialogueArea.text = currentLine.line;
                StartCoroutine(SpacebarCooldown());
                isTyping = false;
            }
            else if (currentLine != null && currentLine.choices.Count > 0)
            {
                StartCoroutine(SpacebarCooldown());
                // If choices exist, show the panel before continuing
                ShowChoices(currentLine.choices);
                if (choicePanel.activeSelf)
                {
                    // Prevent skipping choices
                    return;
                }
            }
            else
            {
                StartCoroutine(SpacebarCooldown());
                DisplayNextDialogueLine();
            }
        }
    }
    IEnumerator SpacebarCooldown()
    {
        canPressSpace = false; // Disable Spacebar input
        yield return new WaitForSeconds(3.0f); // Adjust the delay as needed
        canPressSpace = true; // Enable Spacebar input again
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        DialoguePanel.SetActive(true);
        lines.Clear();

        // Disable player movement if script is assigned
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }



    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    // Call this after text finishes displaying
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";

        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // ✅ If choices exist, show the choice panel **before** allowing further progression
        if (dialogueLine.choices.Count > 0)
        {
            ShowChoices(dialogueLine.choices);
        }
    }


    void ShowChoices(List<DialogueChoice> choices)
    {
        choicePanel.SetActive(true);

        // Get all choice buttons (assuming they are manually assigned in Unity)
        Button[] choiceButtons = choicePanel.GetComponentsInChildren<Button>(true); // Include inactive buttons

        if (choiceButtons.Length < choices.Count)
        {
            Debug.LogError("Not enough buttons for choices! Make sure you have enough buttons in the UI.");
            return;
        }

        // Hide all buttons first
        foreach (Button btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }

        Button firstButton = null;
        for (int i = 0; i < choices.Count; i++)
        {
            Button choiceButton = choiceButtons[i];
            choiceButton.gameObject.SetActive(true); // Ensure button is active
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choices[i].choiceText;

            // Find the TimeIcon inside this button
            Transform timeIconTransform = choiceButton.transform.Find("TimeIcon");
            if (timeIconTransform != null)
            {
                Image timeIcon = timeIconTransform.GetComponent<Image>();
                timeIcon.gameObject.SetActive(choices[i].timeCost > 0); // Show only if timeCost > 0
            }

            // Properly capture choice variable
            DialogueChoice choice = choices[i];
            choiceButton.onClick.RemoveAllListeners();
            choiceButton.onClick.AddListener(() => ChooseOption(choice));

            if (firstButton == null)
            {
                firstButton = choiceButton;
            }
        }

        // Automatically highlight first button
        if (firstButton != null)
        {
            firstButton.Select();
        }
    }



    public void ChooseOption(DialogueChoice choice)
    {
        choicePanel.SetActive(false);

        // ⏳ Only deduct time if choice has a time cost (1 clock = 2 hours)
        if (choice.timeCost > 0)
        {
            StartCoroutine(FadeEffect());
            //TimeSystem.Instance.DeductTime(choice.timeCost * 2);
        }

        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue);
        }
        else
        {
            DisplayNextDialogueLine();
        }
    }


    void EndDialogue()
    {
        isDialogueActive = false;
        DialoguePanel.SetActive(false);

        // Re-enable player movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        // Notify the trigger that dialogue has ended
        DialogueTrigger currentTrigger = FindObjectOfType<DialogueTrigger>();
        if (currentTrigger != null)
        {
            currentTrigger.EndDialogue();
        }
    }

}
