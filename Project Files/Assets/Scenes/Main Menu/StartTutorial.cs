using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class StartTutorial : MonoBehaviourPunCallbacks
{
    public string DestinationScene;
    private string GameID;
    public int MaxPlayers;
    private bool isTutorial;
    public void CreateTutorialRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            isTutorial = true;
            GameID = PhotonNetwork.AuthValues.UserId; //Making a unique gameID for each game session
            Debug.Log($"Successfully generated game ID: {GameID}");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)MaxPlayers;

            string playerName = "Host";
            PhotonNetwork.NickName = playerName;
            Debug.Log($"New player's name: {playerName}");

            PhotonNetwork.CreateRoom(GameID, roomOptions);
        }
        else
        {
            Debug.LogError("Cannot create room- not connected to the Master Server.");
        }
    }
    public override void OnJoinedRoom()
    {
        if (isTutorial)
        {
            SceneManager.LoadScene(DestinationScene);
        }
    }
}
