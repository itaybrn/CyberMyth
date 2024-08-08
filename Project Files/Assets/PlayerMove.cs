using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable
{
    public float moveSpd;
    public float str;
    public Rigidbody2D myRigidbody2D;

    //Key Bindings
    public KeyCode MoveLeft;
    public KeyCode MoveRight;
    public KeyCode Jump;

    //Name of player
    public GameObject CanvasName;
    public TextMeshProUGUI Username;

    //Included for handling multiplayer
    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private Quaternion networkedRotation;
    private float lerpSpeed = 10f;
    private double lastUpdateTime;

    private PhotonView photonView;

    void Awake()
    {
        // Initialize the photonView reference
        photonView = GetComponent<PhotonView>();

        // Log to ensure it's properly initialized
        if (photonView == null)
            Debug.LogError("PhotonView is null in PlayerMove.Awake()");
        else
        {
            photonView.name = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log($"PhotonView name: {photonView.name}, PhotonView ID: {photonView.ViewID}");

            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;

            myRigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            myRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            int playerLayer = LayerMask.NameToLayer("Players");
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer);

            if (photonView.IsMine)
                Debug.Log($"IsMine is true for {photonView.name} - {photonView.ViewID}");
            else
                Debug.Log($"IsMine is false for {photonView.name} - {photonView.ViewID}");
        }
    }

    void Start() //CHANGING NOW
    {
        if (photonView.IsMine)
        {
            // Notify GameManager that this player is ready
            StartCoroutine(NotifyGameManagerWhenReady());
        }
    }

    private IEnumerator NotifyGameManagerWhenReady()
    {
        // Wait until Awake is completed and any necessary initialization is done
        yield return new WaitForSeconds(0.1f); // Small delay to ensure initialization is complete

        if (PhotonNetwork.IsMasterClient)
        {
            bool readySignalSent = PhotonNetwork.RaiseEvent(1, null, RaiseEventOptions.Default, SendOptions.SendReliable);
            if (!readySignalSent) Debug.LogError("Error! Sending ready signal failed!");
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
            lastUpdateTime = PhotonNetwork.Time;
        }
        else
        {
            float lag = Mathf.Max(0, (float)(PhotonNetwork.Time - lastUpdateTime));
            Vector3 targetPosition = networkedPosition + networkedVelocity * lag;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkedRotation, Time.deltaTime * lerpSpeed);
        }
    }

    public void SetUsername()
    {
        if (photonView.IsMine)
        {
            string username = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log($"SetUsername was called for {username}!");
            photonView.RPC("RPC_SetUsername", RpcTarget.AllBuffered, username);
        }
    }


    [PunRPC]
    void RPC_SetUsername(string usernameString)
    {
        Debug.Log($"[RPC_SetUsername] RPC called with: {usernameString} for {photonView.Owner.NickName} - {photonView.ViewID}");
        if (Username != null)
        {
            // Update the username display based on whether the PhotonView belongs to the local player
            Username.text = usernameString;
            Username.color = photonView.IsMine ? Color.yellow : Color.white;
        }
        else
        {
            Debug.LogError("[RPC_SetUsername] Username TextMeshProUGUI component is not assigned!");
        }
    }

    private void HandleInput()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(MoveRight) && !Input.GetKey(MoveLeft))
            movement = Vector2.right * moveSpd;
        else if (Input.GetKey(MoveLeft) && !Input.GetKey(MoveRight))
            movement = Vector2.left * moveSpd;

        myRigidbody2D.velocity = new Vector2(movement.x, myRigidbody2D.velocity.y);

        if (Input.GetKeyDown(Jump))
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, str);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(myRigidbody2D.velocity);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkedPosition = (Vector3)stream.ReceiveNext();
            networkedVelocity = (Vector3)stream.ReceiveNext();
            networkedRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
