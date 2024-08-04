using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToPhotonCloud : MonoBehaviourPunCallbacks
{
    public string MainMenuScene;

    void Start()
    {
        // Connect to the Photon Master Server
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LoadLevel(MainMenuScene);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
    }

}
