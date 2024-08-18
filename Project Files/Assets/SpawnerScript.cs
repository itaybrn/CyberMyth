using UnityEngine;
using Photon.Pun;

public class SpawnerScript : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady) // Check if connected to Photon
        {
            Vector3 spawnPosition = new Vector3(Random.Range(4.5f, 16), 15.5f, 0);
            PhotonNetwork.Instantiate("CollegeStudent/Demo/CollegeStudent Variant", spawnPosition, Quaternion.identity);

            Debug.Log($"Player created for {PhotonNetwork.LocalPlayer.NickName}");
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected!");
        }
    }

}
