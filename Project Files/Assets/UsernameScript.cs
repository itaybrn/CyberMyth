using UnityEngine;

public class UsernameScript : MonoBehaviour
{
    public Transform playerTransform;
    public Transform textTransform;

    private void Update()
    {
        if (playerTransform.localScale.x < 0) //player faces left
        {
            if (textTransform.localScale.x > 0)
                textTransform.localScale = new Vector3(-1 * textTransform.localScale.x, textTransform.localScale.y, textTransform.localScale.z);
        }
        else //player faces right
        {
            if (textTransform.localScale.x < 0)
                textTransform.localScale = new Vector3(-1 * textTransform.localScale.x, textTransform.localScale.y, textTransform.localScale.z);
        }
    }
}