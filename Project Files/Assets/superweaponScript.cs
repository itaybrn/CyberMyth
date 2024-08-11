using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class superweaponScript : MonoBehaviour
{
    public int killCoordinateX;
    public float projectileSpeed;
    public Manipulator manipulator;
    // Start is called before the first frame update
    void Start()
    {
        GameObject manipulatorObject = GameObject.FindWithTag("Manipulator"); // Assuming the manipulator GameObject has the tag "Manipulator"
        if (manipulatorObject != null)
            manipulator = manipulatorObject.GetComponent<Manipulator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x > killCoordinateX)
            Destroy(gameObject);
        else
            transform.position = transform.position + Vector3.right * projectileSpeed * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Time Manipulable"))
        {
            this.manipulator.destroyObject(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
