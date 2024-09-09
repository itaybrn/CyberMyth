using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;
using PowerUpCommands;

[RequireComponent(typeof(PhotonView))]
public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public float moveSpd;
    public float str;
    public Rigidbody2D myRigidbody2D;
    public VoiceCommand recorder;
    Manipulator manipultor;

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
        //Initialize settings
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        myRigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        myRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        int playerLayer = LayerMask.NameToLayer("Players");
        Physics2D.IgnoreLayerCollision(playerLayer, playerLayer);

        //Initialize the photonView reference
        photonView = GetComponent<PhotonView>();
        if (photonView == null) 
            Debug.LogError("PhotonView is null in PlayerMove.Awake()");
        else
            photonView.name = PhotonNetwork.LocalPlayer.NickName;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(NotifyGameManagerWhenReady());
            recorder = gameObject.GetComponent<VoiceCommand>();
            manipultor = FindAnyObjectByType<Manipulator>();
        }
    }

    private IEnumerator NotifyGameManagerWhenReady()
    {
        yield return new WaitForSeconds(0.1f); //Small delay to ensure initialization is complete

        if (photonView.IsMine)
        {
            var properties = new ExitGames.Client.Photon.Hashtable { { "IsReady", true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            PowerUpCommand command = recorder.Run(0);
            if (command != null)
                sendCommandToManipulator(command);

            HandleInput();
            lastUpdateTime = PhotonNetwork.Time;
        }
        else
        {
            float lag = Mathf.Max(0, (float)(PhotonNetwork.Time - lastUpdateTime));
            Vector3 targetPosition = networkedPosition + networkedVelocity * lag;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);;
        }
    }

    private void sendCommandToManipulator(PowerUpCommand command)
    {
        PhotonView manPhotonView = this.manipultor.GetComponent<PhotonView>();

        if(command != null)
            manPhotonView.RPC("savePowerUp", RpcTarget.All, command);
    }

    public void SetUsername()
    {
        if (photonView.IsMine)
        {
            string username = PhotonNetwork.LocalPlayer.NickName;
            photonView.RPC("RPC_SetUsername", RpcTarget.AllBuffered, username);
        }
    }


    [PunRPC]
    void RPC_SetUsername(string usernameString)
    {
        if (Username != null)
        {
            Username.text = usernameString;
            Username.color = photonView.IsMine ? Color.yellow : Color.white;
        }
        else
            Debug.LogError("[RPC_SetUsername] Username TextMeshProUGUI component is not assigned!");
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
            stream.SendNext(transform.position);
        else
            networkedPosition = (Vector3)stream.ReceiveNext();
    }
}
