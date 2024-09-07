using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenuUI;
    public Button restartButton; // Reference to the Restart Level button
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        // Check if the current player is the Master Client and set the button's interactable state
        if (!PhotonNetwork.IsMasterClient)
        {
            restartButton.interactable = false; // Disable the button for non-hosts
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            restartButton.interactable = true; // Enable if became host

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void LoadMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your menu scene name
    }

    public void RestartLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReloadScene", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }
}
