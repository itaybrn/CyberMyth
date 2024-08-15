using UnityEngine;
using Photon.Pun;

public class ConnectToPhotonCloud : MonoBehaviourPunCallbacks
{
    public string MainMenuScene;

    void Start()
    {
        Debug.Log("Connecting to Master Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
        PhotonNetwork.LoadLevel(MainMenuScene);
    }

}
