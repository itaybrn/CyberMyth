using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenuUI;
    public Button restartButton;
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        if (!PhotonNetwork.IsMasterClient)
            restartButton.interactable = false;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            restartButton.interactable = true;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
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
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("ReloadScene", RpcTarget.All);
    }

    [PunRPC]
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }
}
