using UnityEngine;
using Photon.Pun;
using ClearSky;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int numOfPlayers;
    private int playersReady = 0;
    private bool allPlayersReady = false;

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
        if (!allPlayersReady)
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

            if (playersReady >= numOfPlayers && !allPlayersReady)
            {
                allPlayersReady = true;
                Debug.Log("All players are ready. Proceeding with game setup.");
                TriggerUsernameDisplay();
            }
        }
    }

    [PunRPC]
    private void TriggerUsernameDisplay()
    {
        foreach (var player in FindObjectsOfType<DemoCollegeStudentController>())
            player.SetUsername();
    }
}
