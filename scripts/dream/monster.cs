using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public Animator doorAnimator;           // Reference to the door animator
    public ClockTimer clockTimer;           // Reference to the ClockTimer to get the time
    public int triggerMinute = 30;          // The specific minute to trigger the countdown
    private float countdown = 0f;           // Countdown timer (float for smooth decrement)
    private bool isCountingDown = false;    // Flag to track if countdown has started
    private bool doorIsOpen = false;        // Tracks the state of the door (open or closed)
    public float doorOpenDuration = 2f;     // Duration to wait with the door open (in seconds)

    void Start()
    {
        if (clockTimer == null)
        {
            Debug.LogError("ClockTimer reference is not set on Monster script.");
        }
    }

    void Update()
    {
        int currentMinute = clockTimer.CurrentMinute;
        int currentHour = clockTimer.CurrentHour;

        if (currentHour == clockTimer.endHour && currentMinute == 0)
        {
            isCountingDown = false;
            return;
        }

        if (currentMinute == triggerMinute && !isCountingDown)
        {
            countdown = Random.Range(1f, 5f);
            isCountingDown = true;
        }

        if (isCountingDown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                TriggerDoorAnimation();
                isCountingDown = false;
            }
        }
    }

    void TriggerDoorAnimation()
    {
        if (doorAnimator != null)
        {
            if (doorIsOpen)
            {
                doorAnimator.SetTrigger("CloseDoor");  // Trigger closing animation
            }
            else
            {
                doorAnimator.SetTrigger("OpenDoor");   // Trigger opening animation
                StartCoroutine(WaitAndCloseDoor());    // Start coroutine to wait before closing
            }
            doorIsOpen = !doorIsOpen;
        }
    }

    // Coroutine to wait before closing the door
    private IEnumerator WaitAndCloseDoor()
    {
        yield return new WaitForSeconds(doorOpenDuration);  // Wait for specified duration
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("CloseDoor");  // Trigger the closing animation
            doorIsOpen = false;                    // Ensure door state is updated
        }
    }
}
