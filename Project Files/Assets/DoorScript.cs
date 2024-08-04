using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private PPScript[] pressurePlates;
    private bool isOpen;

    void Start()
    {
        // Find all PressurePlate instances in the scene
        pressurePlates = FindObjectsOfType<PPScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AreAllPlatesPressed())
            this.isOpen = true;
    }

    bool AreAllPlatesPressed()
    {
        foreach (PPScript plate in pressurePlates)
        {
            if (!plate.IsPressed)
                return false;
        }
        return true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the door's trigger collider
        if (other.CompareTag("Player") && this.isOpen)
        {
            // Destroy the player object
            Destroy(other.gameObject);

            // Notify LevelTransitioner that a player has exited
            LevelTransitioner.instance.PlayerExited();
        }
    }
}
