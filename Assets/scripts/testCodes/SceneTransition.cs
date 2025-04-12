using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;       // The name of the scene to load
    public GameObject location;      // Reference to the location name UI element
    public Animator animator;        // Reference to the Animator component

    private bool playerInRange = false;
    private bool interacting = false;

    void Start()
    {
        location.SetActive(false);
    }

    void Update()
    {
        // Show location name when the player is near the door
        if (playerInRange && !interacting)
        {
            location.SetActive(true);

            // Start door animation and scene transition when "E" is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                interacting = true;
                location.SetActive(false);  // Hide the location name
                animator.SetTrigger("OpenDoor");  // Start the door opening animation
            }
        }
    }

    // This function will be called by the Animation Event at the end of the door animation
    public void LoadSceneAfterAnimation()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            location.SetActive(false);  // Hide the location name when the player moves away
        }
    }
}
