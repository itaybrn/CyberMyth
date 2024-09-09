using UnityEngine;
using Photon.Pun;
using TMPro;

public class JoinGameScript : MonoBehaviourPunCallbacks
{
    public TMP_InputField InputJoin;
    public TMP_InputField PlayerName;
    public TextMeshProUGUI ErrorText;
    public string DestinationScene;

    private const short ERR_LOBBY_FULL = 32765; //Error code
    private const short ERR_LOBBY_NOT_FOUND = 32758; //Error code

    public void JoinRoom()
    {
        if (PlayerName.text == "")
        {
            ErrorText.text = "Please choose a nickname.";
            return;
        }

        PlayerPrefs.SetString("PlayerName", PlayerName.text);
        PhotonNetwork.NickName = PlayerName.text;
        PhotonNetwork.JoinRoom(InputJoin.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(DestinationScene);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError($"Failed to join room: {message} (Error code: {returnCode})");

        switch (returnCode)
        { 
            case ERR_LOBBY_FULL:
                ErrorText.text = "Room unavailable: Max players reached.";
                break;
            case ERR_LOBBY_NOT_FOUND:
                ErrorText.text = "Room not found.";
                break;
        }
    }
}