using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DoorScript : MonoBehaviour
{
    public Sprite doorOpenTexture;
    private PPScript[] pressurePlates;
    private bool isOpen;
    LevelTransitioner transitioner;

    void Start()
    {
        // Find all PressurePlate instances in the scene
        pressurePlates = FindObjectsOfType<PPScript>();
        transitioner = FindAnyObjectByType<LevelTransitioner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AreAllPlatesPressed())
        {
            Debug.Log("All pressure plates are pressed!");
            if (!isOpen)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = doorOpenTexture;
            }
            isOpen = true;
        }
    }

    bool AreAllPlatesPressed()
    {
        foreach (PPScript plate in pressurePlates)
            if (!plate.IsPressed)
                return false;

        return true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the door's trigger collider
        if (other.CompareTag("Player") && isOpen)
        {
            Debug.Log("Player entered the door: " + other.name);
            // Notify all players that a player has exited
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("PlayerExited", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void PlayerExited(int playerViewID)
    {
        Debug.Log("PlayerExited RPC called with viewID: " + playerViewID);
        // Find the player object using the view ID and destroy it
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        if (player != null)
        {
            Debug.Log("Destroying player: " + player.name);
            Destroy(player);  // Here is the destroy command

            // Notify LevelTransitioner that a player has exited
            transitioner.PlayerExited();
        }
        else
        {
            Debug.Log("Player object not found for viewID: " + playerViewID);
        }
    }
}
