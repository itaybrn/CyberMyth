using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTutorial : MonoBehaviour
{
    public string TutorialScene;

    public void GoToTutorial()
    {
        SceneManager.LoadScene(TutorialScene);
    }
}
