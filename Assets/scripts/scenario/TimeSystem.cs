using UnityEngine;
using TMPro;
using System;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance;

    public int currentHour = 8;  // Starts at 8:00 AM
    public TextMeshProUGUI clockText; // UI to display time

    public GameObject character;

    public GameObject[] otherdialogue;

    public Dialogue endOfDayDialogue; // The Dialogue to be triggered at the end of the day



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        UpdateClockUI();
        character.SetActive(false);
    }

    // 🕒 Deducts time and checks for day-end
    public void DeductTime(int hours)
    {
        currentHour += hours;
        UpdateClockUI();

        if (currentHour == 18)  // If it's 6:00 PM or later
        {
            EndDay();  // Trigger end of day dialogue
        }
    }

    // ⏰ Updates the clock UI
    private void UpdateClockUI()
    {
        string period = currentHour >= 12 ? "PM" : "AM";
        int displayHour = currentHour > 12 ? currentHour - 12 : currentHour;
        if (displayHour == 0) displayHour = 12; // Fix 12 AM/PM formatting

        clockText.text = $"{displayHour}:00 {period}";
    }

    // 🏁 Handles the end-of-day event
    public void EndDay()
    {
        TimeDialogue.Instance.StartDialogue(endOfDayDialogue);
        character.SetActive(true);
        foreach (GameObject dialogue in otherdialogue)
        {
            dialogue.SetActive(false);
        }
    }

}
