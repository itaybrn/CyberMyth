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
