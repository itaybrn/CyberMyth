using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPScript : MonoBehaviour
{
    public Transform plate;             //The transform of the part of the pressure plate that moves
    public float pressDepth = 0.25f;    //The depth the plate should press down
    public float pressSpeed = 2f;       //The speed at which the plate moves
    public float releaseDelay = 0.2f;   //Delay before the plate is released

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private Coroutine releaseCoroutine;

    public bool IsPressed
    {
        get { return this.isPressed; }
    }

    void Start()
    {
        initialPosition = plate.localPosition;
        pressedPosition = initialPosition - new Vector3(0, pressDepth, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
            plate.localPosition = Vector3.Lerp(plate.localPosition, pressedPosition, Time.deltaTime * pressSpeed);
        else
            plate.localPosition = Vector3.Lerp(plate.localPosition, initialPosition, Time.deltaTime * pressSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Time Manipulable"))
        {
            isPressed = true;

            if (releaseCoroutine != null)
            {
                StopCoroutine(releaseCoroutine);
                releaseCoroutine = null;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Time Manipulable"))
            releaseCoroutine = StartCoroutine(ReleasePlateAfterDelay(releaseDelay));
    }

    private IEnumerator ReleasePlateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPressed = false;
    }


}
