using UnityEngine;
using Photon.Pun;

public class NotEnoughPlayers : MonoBehaviour
{
    public GameObject notEnoughPlayersUI;

    void Start()
    {
        notEnoughPlayersUI.SetActive(false);
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            notEnoughPlayersUI.SetActive(true);
    }
}
