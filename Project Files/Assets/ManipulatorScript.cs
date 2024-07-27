using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUpCommands;

public class TMObject
{
    public GameObject obj;
    public BoundedStack<Vector2> positions;
    private int positionsPerSecond;

    public TMObject(GameObject obj, int maxSeconds, int positionsPerSecond)
    {
        this.obj = obj;
        this.positionsPerSecond = positionsPerSecond;
        positions = new BoundedStack<Vector2>(maxSeconds * this.positionsPerSecond);
    }
     public void updatePosition()
    {
        positions.Push(obj.transform.position);
        Debug.Log(obj.transform.position);
    }

    public void rewindPosition(float seconds)
    {
        Vector2 firstPosition = new();
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
            obj.transform.position = firstPosition;
        else
            obj.transform.position = positions.Peek();
    }
}

public class ManipulatorScript : MonoBehaviour
{
    public string layerName;
    public int maxSeconds;
    public int positionsPerSecond;
    public List<TMObject> objects;
    private float timer;
    public float rewindSecs; //will be changed
    VoiceCommand recorder;
    // Start is called before the first frame update
    void Start()
    {
        this.recorder = gameObject.GetComponent<VoiceCommand>();
        this.timer = 0;
        objects = new List<TMObject>();

        GameObject[] objectsInLayer = GameObject.FindGameObjectsWithTag(layerName);
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
        if (this.timer < ((float)1 / this.positionsPerSecond))
            this.timer += Time.deltaTime;
        else
        {
            this.timer = 0;
            foreach (TMObject obj in objects)
                obj.updatePosition();
        }
    }

    public void ExecutePowerUp()
    {
        PowerUpCommand command = recorder.Run();
        if (command != null)
        {
            PowerUp type = command.type;
            switch(command.type)
            {
                case PowerUp.TimeRewind:
                    foreach (TMObject obj in objects)
                        obj.rewindPosition(command.parameter);
                    break;
                default:
                    break;
            }
        }
    }
}
