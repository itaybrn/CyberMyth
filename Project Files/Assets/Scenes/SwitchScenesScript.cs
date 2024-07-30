using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenesScript : MonoBehaviour
{
    public string sceneName;

    public void OnButtonClick()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene(sceneName);
    }
}