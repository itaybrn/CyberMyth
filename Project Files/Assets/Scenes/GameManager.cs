using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Register for events
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        // Initialize scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Initialize game setup
        InitializePlayers();
    }

    private void InitializePlayers()
    {
        Debug.Log($"There are now {PhotonNetwork.CurrentRoom.PlayerCount} / {numOfPlayers} players in the game lobby");
        CheckAllPlayersReady();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered room: {newPlayer.NickName}");
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == numOfPlayers)
        {
            Debug.Log("All players are instantiated. Checking readiness.");
            if (playersReady == numOfPlayers)
            {
                Debug.Log("All players are ready. Proceeding with game setup.");
                // Notify all players that readiness check is complete
                AllPlayersReady();
            }
        }
    }

    private void OnEvent(EventData photonEvent)
    {
        Debug.Log($"Event code: {photonEvent.Code}");
        playersReady++;
        Debug.Log($"Player ready. {playersReady} / {numOfPlayers} players are ready.");

        if (playersReady >= numOfPlayers)
            CheckAllPlayersReady();
    }

    [PunRPC]
    private void AllPlayersReady()
    {
        Debug.Log("All players have been signaled to set their usernames.");
        foreach (var player in FindObjectsOfType<PlayerMove>())
        {
            player.SetUsername();  // Call SetUsername on each player instance
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
