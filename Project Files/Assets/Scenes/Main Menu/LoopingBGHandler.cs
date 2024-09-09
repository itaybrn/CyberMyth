using UnityEngine;

public class LoopingBGHandler : MonoBehaviour
{
    public float speed;
    public float resetPosition; 
    public float startPosition;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y >= resetPosition)
            transform.position = new Vector3(transform.position.x, startPosition, transform.position.z);
    }
}
