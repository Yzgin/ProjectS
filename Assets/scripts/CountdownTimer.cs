using UnityEngine;
using TMPro;
using System.Collections;

public class ClockTimer : MonoBehaviour
{
    public TextMeshProUGUI clockText;  // TextMeshPro for displaying the time
    public Light directionalLight;     // Reference to the directional light
    public Color afternoonLightColor;  // The color for light after 3:00 PM
    public Color eveningLightColor;    // The color for light at 6:00 PM

    public float startYRotation = 15f;    // Initial Y rotation at 10:00 AM
    public float endYRotation = 80f;      // Final Y rotation at 6:00 PM

    public float totalTime = 300f;     // Total time in seconds (5 minutes in real-time)

    private float elapsedTime = 0f;    // Tracks the time passed
    private int startHour = 10;        // Starting time: 10 AM
    //private int endHour = 18;          // Ending time: 6 PM (18:00)

    private void Start()
    {
        StartCoroutine(ClockRoutine());
    }

    private IEnumerator ClockRoutine()
    {
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the time progression (10 AM to 6 PM is an 8-hour window)
            float timeFraction = elapsedTime / totalTime;
            int totalMinutesPassed = Mathf.FloorToInt(timeFraction * (8 * 60));  // Total minutes passed in 8 hours

            // Calculate the current hour and minute
            int currentHour = startHour + (totalMinutesPassed / 60);
            int currentMinute = totalMinutesPassed % 60;

            // Update the clock UI display as HH:MM AM/PM
            string amPm = currentHour >= 12 ? "PM" : "AM";
            int displayHour = (currentHour > 12) ? currentHour - 12 : currentHour;
            if (displayHour == 0) displayHour = 12;  // Handle "00" hour (should show 12 AM or PM)
            string timeString = $"{displayHour:D2}:{currentMinute:D2} {amPm}";
            clockText.text = timeString;

            // Calculate the overall light change progress from 10:00 AM to 6:00 PM
            float lightChangeProgress = timeFraction; // This will vary from 0 to 1 as time progresses from 10 AM to 6 PM

            // Smoothly change the light color and rotation
            directionalLight.color = Color.Lerp(afternoonLightColor, eveningLightColor, lightChangeProgress);
            float currentYRotation = Mathf.Lerp(startYRotation, endYRotation, lightChangeProgress);
            directionalLight.transform.rotation = Quaternion.Euler(50f, currentYRotation, 0f);  // Assuming a 50° tilt angle for X-axis

            yield return null;  // Wait for the next frame
        }

        // Ensure the clock stops at exactly 6:00 PM and final light color and rotation are set
        clockText.text = "06:00 PM";
        directionalLight.color = eveningLightColor;
        directionalLight.transform.rotation = Quaternion.Euler(50f, endYRotation, 0f);  // Set the final rotation to Y = 80°
    }
}
