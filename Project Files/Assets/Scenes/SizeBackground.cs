using UnityEngine;

public class ResizeBackground : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        //Get camera height & width
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        //Get the sprite's bounds
        Vector2 spriteSize = sr.sprite.bounds.size;

        //Scale the sprite to match camera size & center it
        transform.localScale = new Vector3(width / spriteSize.x, height / spriteSize.y, 1);
        transform.position = cam.transform.position;
        transform.position += new Vector3(0, 0, 10); // Move it slightly behind the camera (or adjust as needed)
    }
}
