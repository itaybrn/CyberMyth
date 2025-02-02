using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Button startGameButton;
    public int MinPlayers;
    public int MaxPlayers;

    public GameObject playerNamePrefab;
    public Transform playerNameContainer;

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

        ExitGames.Client.Photon.Hashtable expectedPlayerCount = new ExitGames.Client.Photon.Hashtable();
        expectedPlayerCount.Add("ExpectedPlayerCount", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.CurrentRoom.SetCustomProperties(expectedPlayerCount);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //If player has a name object
            if (playerNameObjects.TryGetValue(player.ActorNumber, out GameObject existingNameObject))
            {
                TextMeshProUGUI textComponent = existingNameObject.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                    textComponent.text = player.NickName;
                else
                    Debug.LogError("TextMeshProUGUI component not found on existing name object.");
            }
            else
            {
                GameObject newNameObject = Instantiate(playerNamePrefab, playerNameContainer);
                TextMeshProUGUI textComponent = newNameObject.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                    textComponent.text = player.NickName;
                else
                    Debug.LogError("TextMeshProUGUI component not found on prefab.");

                playerNameObjects.Add(player.ActorNumber, newNameObject);
            }
        }

        //Remove name objects for players who have left the room
        List<int> actorNumbersToRemove = new List<int>();
        foreach (var keyAndValue in playerNameObjects)
        {
            if (!PhotonNetwork.CurrentRoom.Players.ContainsKey(keyAndValue.Key))
            {
                Destroy(keyAndValue.Value);
                actorNumbersToRemove.Add(keyAndValue.Key);
            }
        }

        //Clean up the dictionary
        foreach (int actorNumber in actorNumbersToRemove)
            playerNameObjects.Remove(actorNumber);
    }

    private void CheckStartButton()
    {
        Debug.Log("Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= MinPlayers;
    }
}
