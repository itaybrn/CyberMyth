using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}
