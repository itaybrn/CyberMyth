using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene("SampleScene");
    }
}
