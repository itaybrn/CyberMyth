using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreateGameScript : MonoBehaviourPunCallbacks
{
    public string DestinationScene;
    public string GameID;
    public int MaxPlayers;
    private bool isTutorial;

    public void CreateGameRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            isTutorial = false;
            Debug.Log("IN CREATE-GAME-SCRIPT");

            GameID = System.Guid.NewGuid().ToString();
            GameID = GameID.Substring(0, 4).ToUpper();
            Debug.Log($"Successfully generated game ID: {GameID}");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)MaxPlayers;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "GameID", GameID } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "GameID" }; // Include GameID in the lobby property list if needed

            PlayerPrefs.SetString("PlayerName", "Host");

            PhotonNetwork.CreateRoom(GameID, roomOptions);
        }
        else
        {
            Debug.LogError("Cannot create room. Not connected to the Master Server.");
        }
    }

    public override void OnJoinedRoom()
    {
        if (!isTutorial)
        {
            PhotonNetwork.LoadLevel(DestinationScene);
        }
        
    }
}
