using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class StartTutorial : MonoBehaviourPunCallbacks
{
    private bool isTutorial;
    public void CreateTutorialRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            isTutorial = true;
            Debug.Log("IN START-TUTORIAL");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)1;

            PlayerPrefs.SetString("PlayerName", "Host");
            PhotonNetwork.CreateRoom("Tutorial", roomOptions);
        }
        else
        {
            Debug.LogError("Cannot create room. Not connected to the Master Server.");
        }
    }
    public override void OnJoinedRoom()
    {
        if (isTutorial)
        {
            Debug.Log($"!!! Loading scene: Tutorial1");
            SceneManager.LoadScene("Tutorial1");
        }
    }
}
