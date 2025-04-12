using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Toy_system : MonoBehaviour
{
    public float Countdown = 30f; // Total countdown time in seconds
    public float MaxTime = 30f; // Maximum time the timer can have
    public float WindUpRate = 3f; // Time added per second while holding "E"
    public Slider Timer; // UI Slider for the countdown timer
    public Animator Toy_animation; // Animator for toy animations

    private bool winding = false; // Flag to check if winding is in progress
    private bool isHeld = false; // Flag to track if the toy is held
    private bool isCountdownActive = false; // Flag to track if the countdown is active

    void Start()
    {
        // Initialize the slider and start the countdown immediately
        if (Timer != null)
        {
            Timer.maxValue = MaxTime;
            Timer.value = Countdown;
        }
        isCountdownActive = true; // Start the countdown immediately when the game starts
    }

    void Update()
    {
        // Handle the countdown if it's active
        if (isCountdownActive && Countdown > 0)
        {
            Countdown -= Time.deltaTime;

            if (Countdown <= 0)
            {
                Countdown = 0;
                // Don’t stop countdown completely
                Debug.Log("Countdown reached zero!");
            }

            if (Timer != null)
            {
                Timer.value = Countdown;
            }
        }

        if (Countdown <= 0)
        {
            Countdown = 0;
            EndCountdown();
        }

        // Allow winding up by holding "E" only if the toy is held
        if (isHeld)
        {
            if (Input.GetKey(KeyCode.E)) // Only add time while holding "E"
            {
                if (!winding)
                {
                    StartWinding();
                }
            }
            else
            {
                StopWinding(); // Stop the winding animation when "E" is released
            }
        }
    }

    public void SetHeldState(bool state)
    {
        // Update the held state when the toy is picked up or dropped
        isHeld = state;

        if (state)
        {
            StartCountdown(); // Start the countdown when the toy is picked up
        }
    }

    private void StartWinding()
    {
        if (Toy_animation != null)
        {
            Toy_animation.SetBool("isWinding", true);

        }
        winding = true;
        StartCoroutine(WindUpTimer());
    }

    private void StopWinding()
    {
        if (Toy_animation != null)
        {
            Toy_animation.SetBool("isWinding", false);
        }
        winding = false;

    }

    private IEnumerator WindUpTimer()
    {
        while (winding && Countdown < MaxTime)
        {
            AddTime(1f);  // Add exactly 1 second every time the loop runs
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void AddTime(float time)
    {
        Countdown += time;

        // Clamp the time to ensure it does not exceed the maximum allowed time
        if (Countdown > MaxTime)
        {
            Countdown = MaxTime;
        }

        if (Timer != null)
        {
            Timer.value = Countdown; // Update the slider value
        }

    }

    private void StartCountdown()
    {
        isCountdownActive = true;
    }

    private void EndCountdown()
    {
        Countdown = 0;
        isCountdownActive = false;

        // Add game over logic here
        Debug.Log("Game Over!");

        // Example: Disable controls, show game over UI, etc.
        // You could trigger an animation, UI panel, or load a new scene here
        // For now, let's just pause the game
        Time.timeScale = 0f;

        // Optional: Show Game Over UI if you have one
        // gameOverPanel.SetActive(true);
    }
}
