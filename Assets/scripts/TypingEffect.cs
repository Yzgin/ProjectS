using UnityEngine;
using TMPro;
using System.Collections;

public class TypingEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f;
    public TextMeshProUGUI textMeshPro;
    public GameObject yesButton; // Reference to the YES button
    public GameObject noButton;  // Reference to the NO button

    private string fullText;

    private void Start()
    {
        fullText = textMeshPro.text;
        textMeshPro.text = ""; // Clear the initial text
        StartCoroutine(TypeText());
        yesButton.SetActive(false);
        noButton.SetActive(false);
    }

    private IEnumerator TypeText()
    {
        foreach (char letter in fullText.ToCharArray())
        {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // After the typing effect is complete, activate the buttons
        yesButton.SetActive(true);
        noButton.SetActive(true);
    }
}
