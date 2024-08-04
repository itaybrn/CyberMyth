using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{
    public static LevelTransitioner instance;
    private int totalPlayers;
    private int playersExited;
    public string nextSceneName;

    void Awake()
    {
        // Ensure there's only one instance of LevelTransitioner
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        totalPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        playersExited = 0;
    }

    public void PlayerExited()
    {
        playersExited++;
        if (playersExited >= totalPlayers)
        {
            //SceneManager.LoadScene(nextSceneName);
            Debug.LogWarning("Should transition to next level");
        }
    }
}
