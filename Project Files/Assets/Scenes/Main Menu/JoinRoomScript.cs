using Photon.Pun;
using TMPro;

public class JoinRoomScript : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputJoin;
    public string DestinationScene;

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputJoin.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(DestinationScene);
    }
}
