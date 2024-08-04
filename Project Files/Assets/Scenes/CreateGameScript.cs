using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateGameScript : MonoBehaviourPunCallbacks
{
    public string DestinationScene;
    public string GameID;
    public int MaxPlayers;

    //void Start()
    //{
    //    // Connect to the Photon Master Server
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Master Server.");
    //}

    public void CreateGameRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            GameID = System.Guid.NewGuid().ToString();
            GameID = GameID.Substring(0, 4).ToUpper();
            Debug.Log($"Successfully generated game ID: {GameID}");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = MaxPlayers;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "GameID", GameID } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "GameID" }; // Include GameID in the lobby property list if needed

            PhotonNetwork.CreateRoom(GameID, roomOptions);
        }
        else
        {
            Debug.LogError("Cannot create room. Not connected to the Master Server.");
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(DestinationScene);
    }
}
