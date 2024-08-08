using UnityEngine;
using Photon.Pun;

public class SpawnerScript : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady) // Check if connected to Photon
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-7.75f, -7), -2.75f, 0);
            PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

            Debug.Log($"Player created for {PhotonNetwork.LocalPlayer.NickName}");
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected!");
        }
    }

}
