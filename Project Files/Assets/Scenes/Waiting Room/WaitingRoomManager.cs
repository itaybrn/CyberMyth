using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Button startGameButton;
    public int MinPlayers;
    public int MaxPlayers;

    public GameObject playerNamePrefab;
    public Transform playerNameContainer;

    public string GameScene;
    public TextMeshProUGUI gameIDText;
    public TextMeshProUGUI statusText;

    private Dictionary<int, GameObject> playerNameObjects = new Dictionary<int, GameObject>();

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameID"))
        {
            string gameID = PhotonNetwork.CurrentRoom.CustomProperties["GameID"].ToString();
            gameIDText.text = "Game ID: " + gameID;
            statusText.text = "Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + MaxPlayers;

            startGameButton.interactable = false;

            string playerName = PlayerPrefs.GetString("PlayerName");
            PhotonNetwork.NickName = playerName;
            Debug.Log($"New player's name: {playerName}");

            UpdatePlayerList();
        }
        else
        {
            Debug.LogError("Failed to retrieve Game ID.");
        }
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected to Master Server.";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)MaxPlayers };
        PhotonNetwork.JoinOrCreateRoom("WaitingRoom", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
        CheckStartButton();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
        CheckStartButton();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        CheckStartButton();
    }

    void UpdatePlayerList()
    {
        statusText.text = $"Players in room: {PhotonNetwork.CurrentRoom.PlayerCount} / {MaxPlayers}";

        // Clear existing player name objects
        foreach (GameObject obj in playerNameObjects.Values)
        {
            Destroy(obj);
        }
        playerNameObjects.Clear();

        // Create new player name objects
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject newNameObject = Instantiate(playerNamePrefab, playerNameContainer);
            TextMeshProUGUI textComponent = newNameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = player.NickName;
                Debug.Log($"Setting name for {player.NickName}");
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on prefab.");
            }
            playerNameObjects.Add(player.ActorNumber, newNameObject);
        }
    }

    void CheckStartButton()
    {
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= MinPlayers;
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(GameScene);
        }
    }
}
