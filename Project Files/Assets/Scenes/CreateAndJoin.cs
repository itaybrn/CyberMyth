using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_CreateGame;
    public TMP_InputField input_JoinGame;

    public void CreateGameRoom()
    {
        PhotonNetwork.CreateRoom(input_CreateGame.text);
    }

    public void JoinGameRoom()
    {
        PhotonNetwork.JoinRoom(input_JoinGame.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GamePlay");
    }
}
