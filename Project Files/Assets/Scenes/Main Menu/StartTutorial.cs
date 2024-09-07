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

            //PlayerPrefs.SetString("PlayerName", "Host");
            string playerName = "Host";
            PhotonNetwork.NickName = playerName;
            Debug.Log($"New player's name: {playerName}");

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
            SceneManager.LoadScene("Tutorial1");
        }
    }
}
