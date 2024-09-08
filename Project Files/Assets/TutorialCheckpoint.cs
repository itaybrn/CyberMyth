using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{
    public GameObject speechBubblePrefab1; 
    public GameObject speechBubblePrefab2 = null; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Ensure the player is tagged "Player"
        {
            Debug.Log("Player detected by checkpoint.");

            // Find and destroy all existing speech bubbles in the scene
            GameObject[] existingSpeechBubbles = GameObject.FindGameObjectsWithTag("Speechbubble");

            foreach (GameObject bubble in existingSpeechBubbles)
            {
                Debug.Log("Destroying existing speech bubble.");
                Destroy(bubble);
            }

            // Instantiate a new speech bubble
            GameObject newBubble = Instantiate(speechBubblePrefab1);
            if (speechBubblePrefab2 != null)
            {
                GameObject newBubble2 = Instantiate(speechBubblePrefab2);
            }

            if (newBubble == null)
            {
                Debug.LogError("Failed to instantiate speech bubble prefab.");
                return;
            }
        }
    }
}
