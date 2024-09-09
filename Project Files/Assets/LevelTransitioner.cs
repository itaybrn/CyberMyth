using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LevelTransitioner : MonoBehaviour
{
    private int totalPlayers;
    private int playersExited;
    public string nextSceneName;
    public bool isFinalLevel;

    void Start()
    {
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        playersExited = 0;
    }

    public void PlayerExited()
    {
        playersExited++;
        if (playersExited >= totalPlayers)
        {
            if (isFinalLevel)
                PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(nextSceneName);
            Debug.LogWarning("Should transition to next level");
        }
    }
}

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Photon.Pun;

//public class LevelTransitioner : MonoBehaviour
//{
//    private int totalPlayers;
//    private int playersExited;
//    public string nextSceneName;
//    public bool isFinalLevel;

//    void Start()
//    {
//        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
//        playersExited = 0;
//    }

//    public void PlayerExited()
//    {
//        playersExited++;
//        if (playersExited >= totalPlayers)
//        {
//            if (isFinalLevel)
//                PhotonNetwork.LeaveRoom();
//            SceneManager.LoadScene(nextSceneName);
//            Debug.LogWarning("Should transition to next level");
//        }
//    }
//}