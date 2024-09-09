using UnityEngine;
using Photon.Pun;

public class DoorScript : MonoBehaviour
{
    public Sprite doorOpenTexture;
    private PPScript[] pressurePlates;
    private bool isOpen;
    LevelTransitioner transitioner;

    void Start()
    {
        pressurePlates = FindObjectsOfType<PPScript>();
        transitioner = FindAnyObjectByType<LevelTransitioner>();
    }

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
        if (other.CompareTag("Player") && isOpen)
        {
            Debug.Log("Player entered the door: " + other.name);
            
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("PlayerExited", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void PlayerExited(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        if (player != null)
        {
            Debug.Log("Destroying player: " + player.name);
            Destroy(player);

            transitioner.PlayerExited(); //Notify LevelTransitioner that a player has exited
        }
        else
        {
            Debug.Log("Player object not found for viewID: " + playerViewID);
        }
    }
}
