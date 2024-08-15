using UnityEngine;

public class LoopingBGHandler : MonoBehaviour
{
    public float speed; // Speed of the background movement
    public float resetPosition; // Position at which the background resets
    public float startPosition; // Starting position for the background

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Store the initial position of the background
    }

    void Update()
    {
        // Move the background to the left
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // If the background moves past the reset position, reset it to the start position
        if (transform.position.y >= resetPosition)
        {
            transform.position = new Vector3(transform.position.x, startPosition, transform.position.z);
        }
    }
}
