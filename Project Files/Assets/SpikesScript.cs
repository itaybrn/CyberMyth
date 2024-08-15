using UnityEngine;

public class SpikesScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                // Call the player's death method
                // player.Die();
            }
        }
    }
}
