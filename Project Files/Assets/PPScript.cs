using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PPScript : MonoBehaviourPunCallbacks
{
    public Transform plate;             
    public float pressDepth = 0.25f;    
    public float pressSpeed = 2f;       
    public float releaseDelay = 0.2f;   

    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private Coroutine releaseCoroutine;

    public bool IsPressed
    {
        get { return isPressed; }
    }

    void Start()
    {
        isPressed = false;
        initialPosition = plate.localPosition;
        pressedPosition = initialPosition - new Vector3(0, pressDepth, 0);
    }

    void Update()
    {
        if (isPressed)
            plate.localPosition = Vector3.Lerp(plate.localPosition, pressedPosition, Time.deltaTime * pressSpeed);
        else
        {
            plate.localPosition = Vector3.Lerp(plate.localPosition, initialPosition, Time.deltaTime * pressSpeed);
            
            if (releaseCoroutine == null)
                releaseCoroutine = StartCoroutine(ReleasePlateAfterDelay(releaseDelay));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Time Manipulable"))
        {
            isPressed = true;
            photonView.RPC("RPC_press", RpcTarget.Others, true);

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
        photonView.RPC("RPC_press", RpcTarget.Others, false);
    }

    [PunRPC]
    void RPC_press(bool pressed)
    {
        isPressed = pressed;
    }
}
