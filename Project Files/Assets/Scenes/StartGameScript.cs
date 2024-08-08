using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{
    public string sceneName;

    public void OnButtonClick()
    {
        Debug.Log("Starting game!");
        SceneManager.LoadScene(sceneName);
    }
}
