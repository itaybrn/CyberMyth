using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameScript : MonoBehaviour
{
    public Transform playerTransform;
    public Transform textTransform; // Reference to the text object's transform

    private void Update()
    {
        // Check if the player is facing left (scale.x < 0) or right (scale.x > 0)
        if (playerTransform.localScale.x < 0)
        {
            if (textTransform.localScale.x > 0)
                textTransform.localScale = new Vector3(-1 * textTransform.localScale.x, textTransform.localScale.y, textTransform.localScale.z);
        }
        else
        {
            if (textTransform.localScale.x < 0)
                textTransform.localScale = new Vector3(-1 * textTransform.localScale.x, textTransform.localScale.y, textTransform.localScale.z); // Keep the text facing right
        }

    }
}
