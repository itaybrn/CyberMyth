using UnityEngine;
using Photon.Pun;
using System.Collections;
using ClearSky;

public class SpawnerScript : MonoBehaviour
{
    public float XCoordLeft, XCoordRight;
    public float YCoord;
    public float delayBeforeStart = 0.5f;
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady) // Check if connected to Photon
        {
            StartCoroutine(DelayedStart());
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected!");
        }
    }

    private IEnumerator DelayedStart()
    {
        // Wait for a specified amount of time
        yield return new WaitForSeconds(delayBeforeStart);

        // Initialize or reset variables here
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Scene has been loaded. Initializing game state for the host.");
        }

        Vector3 spawnPosition = new Vector3(Random.Range(XCoordLeft, XCoordRight), YCoord, 0);
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        TriggerUsernameDisplay();

        Debug.Log($"Player created for {PhotonNetwork.LocalPlayer.NickName}");
    }

    [PunRPC]
    private void TriggerUsernameDisplay()
    {
        foreach (var player in FindObjectsOfType<DemoCollegeStudentController>())
            player.SetUsername();
    }

}
