using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUpCommands;
using Photon.Pun;

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
public class Manipulator : MonoBehaviour
{
    public string tagName;
    public int maxSeconds;
    public int positionsPerSecond;
    public List<TMObject> objects;
    private float timer;
    private float timeStop = 0;
    public GameObject player;
    public string SWShot;
    VoiceCommand recorder;
    public string apiKey;
    public string filePath;
    // Start is called before the first frame update
    void Start()
    {
        this.recorder = gameObject.GetComponent<VoiceCommand>();
        this.timer = 0;
        objects = new List<TMObject>();

        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag(tagName);
        foreach(GameObject obj in objectsInLayer)
        {
            TMObject newObj = new(obj, maxSeconds, positionsPerSecond);
            objects.Add(newObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.SavePositions();
        this.ExecutePowerUp();
    }

    void SavePositions()
    {
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

    public void ExecutePowerUp()
    {
        PowerUpCommand command = recorder.Run();
        if (command != null)
        {
            PowerUp type = command.type;
            switch (command.type)
            {
                case PowerUp.TimeRewind:
                    if (this.timeStop > 0)
                        this.timeStop = 0;
                    foreach (TMObject obj in objects)  
                        obj.rewindPosition(command.parameter);
                    break;
                case PowerUp.TimeStop:
                    this.timeStop = command.parameter;
                    break;
                case PowerUp.Swap:
                    int index = (int)command.parameter;
                    swapPositions(index);
                    break;
                case PowerUp.Superweapon:
                    PhotonNetwork.Instantiate(SWShot, new Vector3(this.player.transform.position.x, this.player.transform.position.y - 1), this.player.transform.rotation);
                    break;
                default:
                    break;
            }
        }
    }

    private void swapPositions(int index)
    {
        if (index > this.objects.Count || index < 1)
            Debug.LogError("index: " + index + " out of range");
        else
        {
            TMObject obj = this.objects[index - 1];
            if (obj.destroyed)
                Debug.LogError("Object is destroyed, can't swap");
            else
            {
                Vector2 temp = this.player.transform.position;
                this.player.transform.position = obj.obj.transform.position;
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
            toBeDestroyed.obj.transform.position = toBeDestroyed.obj.transform.position + Vector3.up * 40;
            toBeDestroyed.destroyed = true;
        }
    }    
}
