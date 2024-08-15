using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LeaveLobbyScript : MonoBehaviour
{
    public string sceneName;

    public void OnButtonClick()
    {
        Debug.Log("Button clicked!");
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(sceneName);
    }
}