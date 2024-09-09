using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{
    public GameObject speechBubblePrefab1; 
    public GameObject speechBubblePrefab2 = null; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  //Assuming the player is tagged "Player"
        {
            Debug.Log("Player detected by checkpoint.");

            GameObject[] existingSpeechBubbles = GameObject.FindGameObjectsWithTag("Speechbubble");
            foreach (GameObject bubble in existingSpeechBubbles)
                Destroy(bubble);


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
