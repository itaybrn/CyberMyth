using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClearSky;


public class YouDiedMessage : MonoBehaviour
{
    public GameObject youDiedMessageUI;
    public GameObject notEnoughPlayersUI;

    void Start()
    {
        youDiedMessageUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DemoCollegeStudentController[] players = FindObjectsOfType<DemoCollegeStudentController>();
        foreach (DemoCollegeStudentController player in players)
        {
            if (!player.isAlive)
            {
                if(notEnoughPlayersUI == null || !notEnoughPlayersUI.activeSelf)
                    youDiedMessageUI.SetActive(true);
            }
        }
    }
}
