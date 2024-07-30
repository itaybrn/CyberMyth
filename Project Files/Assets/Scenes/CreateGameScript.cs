using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateGameScript : MonoBehaviourPunCallbacks
{
    public string DestinationScene;
    public string GameID;

    public void CreateGameRoom()
    {
        GameID = System.Guid.NewGuid().ToString();
        Debug.Log($"Successfully generated game ID: {GameID}");

        PhotonNetwork.CreateRoom(GameID);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("DestinationScene");
    }
}
