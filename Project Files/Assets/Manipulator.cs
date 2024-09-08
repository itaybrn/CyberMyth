using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUpCommands;
using Photon.Pun;
using ClearSky;
using Superweapon;
using TMPro;

public class PositionStatus
{
    public Vector2 position;
    public bool destroyed;

    public PositionStatus(Vector2 pos, bool destroyed)
    {
        this.position = pos;
        this.destroyed = destroyed;
    }
}
public class TMObject
{
    public GameObject obj;
    public BoundedStack<PositionStatus> positions;
    public bool destroyed;
    private int positionsPerSecond;

    public TMObject(GameObject obj, int maxSeconds, int positionsPerSecond)
    {
        this.obj = obj;
        this.positionsPerSecond = positionsPerSecond;
        positions = new BoundedStack<PositionStatus>(maxSeconds * this.positionsPerSecond);
    }
     public void updatePosition()
    {
        PositionStatus curr = new PositionStatus(obj.transform.position, this.destroyed);
        positions.Push(curr);
        Debug.Log(obj.transform.position);
    }

    public void rewindPosition(float seconds)
    {
        PositionStatus firstPosition = new PositionStatus(new Vector2(), false); //temp value
        bool takeFirst = false;
        int toRewind = (int)(seconds * this.positionsPerSecond);
        Debug.Log("rewinding " + toRewind + " positions.");
        for (int i = 0; i < toRewind; i++)
        {
            firstPosition = positions.Pop();
            if (positions.IsEmpty())
            {
                takeFirst = true;
                break;
            }
        }
        if (takeFirst)
        {
            obj.transform.position = firstPosition.position;
            this.destroyed = firstPosition.destroyed;
        }
        else
        {
            PositionStatus curr = positions.Peek();
            obj.transform.position = curr.position;
            this.destroyed = curr.destroyed;
        }
    }
}

[System.Serializable]
public class Manipulator : MonoBehaviourPunCallbacks
{
    public string tagName;
    public int maxSeconds;
    public int positionsPerSecond;
    public List<TMObject> objects;
    private float timer;
    private float timeStop = 0;
    //public GameObject player;
    public string SWShot;
    public string clone;
    //VoiceCommand recorder;
    List<PowerUpCommand> commandsToExecute;
    GameObject[] players;
    //public string apiKey;
    //public string filePath;
    // Start is called before the first frame update
    void Start()
    {
        CommandSerializer.Register();

        this.timer = 0;
        this.objects = new List<TMObject>();
        this.commandsToExecute = new List<PowerUpCommand>();

        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag(tagName);
        this.players = GameObject.FindGameObjectsWithTag("Player");
        Debug.LogWarning("players size: " + this.players.Length);
        foreach (GameObject obj in objectsInLayer)
        {
           SendTextBoxToTMObject(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.players.Length < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            this.players = GameObject.FindGameObjectsWithTag("Player");
            Debug.LogWarning("players size: " + this.players.Length);
        }
        this.SavePositions();
        if(PhotonNetwork.IsMasterClient)
        {
            foreach(PowerUpCommand command in this.commandsToExecute)
                this.ExecutePowerUp(command);
            this.commandsToExecute.Clear();
        }
        
    }

    void SendTextBoxToTMObject(GameObject obj)
    {
        if (obj.GetComponent<HasID>() != null)
        {
            HasID HasIDObject = obj.GetComponent<HasID>();
            if (HasIDObject != null)
            {
                TMObject newObj = new(obj, maxSeconds, positionsPerSecond);
                objects.Add(newObj);
                HasIDObject.DisplayID(objects.Count);
            }
        }
    }

    void SavePositions()
    {
        findNewTMObjects();
        if (this.timeStop > 0)
        {
            foreach (TMObject TMObj in this.objects)
            {
                GameObject obj = TMObj.obj;
                PositionStatus curr = TMObj.positions.Peek();
                obj.transform.position = curr.position;
                TMObj.destroyed = curr.destroyed;
            }
            this.timeStop = Mathf.Max(0, this.timeStop - Time.deltaTime);
        }
        else
        {
            if (this.timer < ((float)1 / this.positionsPerSecond))
                this.timer += Time.deltaTime;
            else
            {
                this.timer = 0;
                foreach (TMObject obj in this.objects)
                    obj.updatePosition();
            }
        }
    }

    private void findNewTMObjects()
    {
        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag(tagName);
        if(objectsInLayer.Length != this.objects.Count)
        {
            foreach(GameObject obj in objectsInLayer)
            {
                bool exists = false;
                foreach(TMObject tmobj in this.objects)
                {
                    if (tmobj.obj == obj)
                        exists = true;
                }

                if(!exists)
                {
                    SendTextBoxToTMObject(obj);
                }
            }
        }
    }

    [PunRPC]
    public void savePowerUp(PowerUp powerUp, float parameter, int playerID)
    {
        Debug.LogWarning("Player index gotten: " + playerID);
        PowerUpCommand newCommand = new PowerUpCommand(powerUp, parameter, playerID);
        if(PhotonNetwork.IsMasterClient)
            this.commandsToExecute.Add(newCommand);
    }

    public void ExecutePowerUp(PowerUpCommand command)
    {
        //PowerUpCommand command = recorder.Run();
        GameObject player = null;
        foreach(GameObject p in this.players)
        {
            if (p.GetComponent<PhotonView>().ViewID == command.playerIndex)
                player = p;
            Debug.LogWarning("Checked ID: " + command.playerIndex + " " + p.GetComponent<PhotonView>().ViewID);
        }

        if (player == null)
            Debug.LogError("Didn't recognize player ID: " + command.playerIndex);
        
        if (command != null)
        {
            PowerUp type = command.type;
            switch (command.type)
            {
                case PowerUp.TimeRewind:
                    photonView.RPC("RPC_rewindTime", RpcTarget.All, command.parameter);
                    break;
                case PowerUp.TimeStop:
                    this.timeStop = command.parameter;
                    break;
                case PowerUp.Swap:
                    swapPositions(command, player);
                    break;
                case PowerUp.Superweapon:
                    PhotonNetwork.Instantiate(SWShot, new Vector3(player.transform.position.x, player.transform.position.y + 5), player.transform.rotation);
                    superweaponScript[] SWShotArray = FindObjectsByType<superweaponScript>(FindObjectsSortMode.None);
                    foreach (superweaponScript shot in SWShotArray)
                    {
                        if (player.transform.localScale.x < 0) //if the player faces left
                            shot.setDirection(true); //SWShot left
                        else
                            shot.setDirection(false); //SWShot right
                    }
                    break;
                case PowerUp.Clone:
                    PhotonNetwork.Instantiate(clone, new Vector3(player.transform.position.x, player.transform.position.y + 4), player.transform.rotation);
                    break;
                default:
                    break;
            }
        }
    }

    private void swapPositions(PowerUpCommand command, GameObject player)
    {
        int index = (int)command.parameter;

        if (index > this.objects.Count || index < 1)
            Debug.LogError("index: " + index + " out of range");
        else
        {
            TMObject obj = this.objects[index - 1];
            if (obj.destroyed)
                Debug.LogError("Object is destroyed, can't swap");
            else
            {
                Vector2 temp = player.transform.position;
                player.GetComponent<DemoCollegeStudentController>().Teleport(obj.obj.transform.position);
                //player.transform.position = obj.obj.transform.position;
                obj.obj.transform.position = temp;
                Debug.LogWarning("Swapped player with object " + index);
            }
        }
    }

    public void destroyObject(GameObject obj)
    {
        TMObject toBeDestroyed = null;
        foreach (TMObject TMObj in this.objects)
        {
            if (TMObj.obj == obj)
                toBeDestroyed = TMObj;
        }
        if (toBeDestroyed == null)
            Debug.LogError("object is not time manipulable");
        else
        {
            toBeDestroyed.obj.transform.position = toBeDestroyed.obj.transform.position + Vector3.down * 250;
            toBeDestroyed.destroyed = true;
        }
    }

    [PunRPC]
    public void RPC_rewindTime(float parameter)
    {
        if (this.timeStop > 0)
            this.timeStop = 0;
        foreach (TMObject obj in objects)
            obj.rewindPosition(parameter);
    }
}
