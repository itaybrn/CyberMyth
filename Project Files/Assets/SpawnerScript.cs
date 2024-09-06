using UnityEngine;
using Photon.Pun;

public class SpawnerScript : MonoBehaviour
{
    public float XCoordLeft, XCoordRight;
    public float YCoord;
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady) // Check if connected to Photon
        {
            Vector3 spawnPosition = new Vector3(Random.Range(XCoordLeft, XCoordRight), YCoord, 0);
            PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

            Debug.Log($"Player created for {PhotonNetwork.LocalPlayer.NickName}");
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected!");
        }
    }

}
