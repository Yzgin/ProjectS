using UnityEngine;
using TMPro;
using System.Collections;

public class ClockTimer : MonoBehaviour
{
    public TextMeshProUGUI clockText;          // TextMeshPro for displaying the time
    public Light directionalLight;             // Reference to the directional light
    public Color nightLightColor;              // The color for light after 11:00 PM
    public Color earlyMorningLightColor;       // The color for light at 3:00 AM

    public float startYRotation = 15f;         // Initial Y rotation at 11:00 PM
    public float endYRotation = 80f;           // Final Y rotation at 3:00 AM
    public float totalTime = 300f;             // Total time in seconds (5 minutes in real-time)

    public int startHour = 23;                 // Starting time (11:00 PM)
    public int endHour = 3;                    // Ending time (3:00 AM)

    private float elapsedTime = 0f;            // Tracks the time passed

    // Expose the current hour and minute for other scripts to access
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    private void Start()
    {
        StartCoroutine(ClockRoutine());
    }

    private IEnumerator ClockRoutine()
    {
        // Calculate the range of hours, considering crossover past midnight
        int hoursRange = (endHour < startHour) ? (endHour + 24 - startHour) : (endHour - startHour);

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the time progression based on the calculated hours range
            float timeFraction = elapsedTime / totalTime;
            int totalMinutesPassed = Mathf.FloorToInt(timeFraction * (hoursRange * 60));  // Total minutes passed in specified hours range

            // Calculate the current hour and minute
            CurrentHour = (startHour + (totalMinutesPassed / 60)) % 24;  // Use modulo 24 to wrap around midnight
            CurrentMinute = totalMinutesPassed % 60;

            // Update the clock UI display as HH:MM AM/PM
            string amPm = CurrentHour >= 12 ? "PM" : "AM";
            int displayHour = (CurrentHour > 12) ? CurrentHour - 12 : CurrentHour;
            if (displayHour == 0) displayHour = 12;  // Handle "00" hour (should show 12 AM or PM)
            string timeString = $"{displayHour:D2}:{CurrentMinute:D2} {amPm}";
            clockText.text = timeString;

            // Calculate the overall light change progress from startHour to endHour
            float lightChangeProgress = timeFraction; // This will vary from 0 to 1 as time progresses from startHour to endHour

            // Smoothly change the light color and rotation
            directionalLight.color = Color.Lerp(nightLightColor, earlyMorningLightColor, lightChangeProgress);
            float currentYRotation = Mathf.Lerp(startYRotation, endYRotation, lightChangeProgress);
            directionalLight.transform.rotation = Quaternion.Euler(50f, currentYRotation, 0f);  // Assuming a 50° tilt angle for X-axis

            yield return null;  // Wait for the next frame
        }

        // Ensure the clock stops at exactly the end time and final light color and rotation are set
        clockText.text = $"{((endHour > 12) ? endHour - 12 : endHour):D2}:00 {(endHour >= 12 ? "PM" : "AM")}";
        directionalLight.color = earlyMorningLightColor;
        directionalLight.transform.rotation = Quaternion.Euler(50f, endYRotation, 0f);  // Set the final rotation to Y = endYRotation
    }
}
