using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Text statusText;
    public Button startGameButton;
    private int maxPlayers;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        startGameButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected to Master Server.";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby. Creating/Joining Room...";
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers };
        PhotonNetwork.JoinOrCreateRoom("WaitingRoom", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room. Waiting for players...";
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
        statusText.text = "Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayers;
    }

    void CheckStartButton()
    {
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
