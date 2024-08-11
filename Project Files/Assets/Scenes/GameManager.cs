using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int numOfPlayers;
    private int playersReady = 0;

    private void Start()
    {
        Debug.Log("GameManager started!");

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("ExpectedPlayerCount"))
            numOfPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["ExpectedPlayerCount"];
        else
            Debug.LogError("ExpectedPlayerCount property is missing!");
    }

    private void Update()
    {
        Debug.Log($"Players ready: {playersReady} / {numOfPlayers}");
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == numOfPlayers)
        {
            if (playersReady < numOfPlayers)
            {
                playersReady = 0;

                foreach (var player in PhotonNetwork.PlayerList)
                    if (player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"])
                        playersReady++;
            }

            if (playersReady >= numOfPlayers)
            {
                Debug.Log("All players are ready. Proceeding with game setup.");
                TriggerUsernameDisplay();
            }
        }
    }

    [PunRPC]
    private void TriggerUsernameDisplay()
    {
        foreach (var player in FindObjectsOfType<PlayerScript>())
            player.SetUsername();
    }
}
