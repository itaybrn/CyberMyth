using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NotEnoughPlayers : MonoBehaviour
{
    public GameObject notEnoughPlayersUI;

    void Start()
    {
        notEnoughPlayersUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            notEnoughPlayersUI.SetActive(true);
    }
}
