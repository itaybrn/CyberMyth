using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace ClearSky
{
    [RequireComponent(typeof(PhotonView))]
    public class DemoCollegeStudentController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float movePower = 10f;
        //public float KickBoardMovePower = 15f;
        public float jumpPower = 20f; //Set Gravity Scale in Rigidbody2D Component to 5

        public float scale_x;
        public float scale_y;
        public float scale_z;

        //Key Bindings
        public KeyCode MoveLeft;
        public KeyCode MoveRight;
        public KeyCode Jump;

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        private int direction = 1;
        bool isJumping = false;
        private bool alive = true;
        //private bool isKickboard = false;

        //Name of player
        public GameObject CanvasName;
        public TextMeshProUGUI Username;

        private PhotonView photonView;
        public Manipulator manipulator;

        void Awake()
        {
            //Initialize settings
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;
            //rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            //rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            int playerLayer = LayerMask.NameToLayer("Players");
            Physics2D.IgnoreLayerCollision(playerLayer, playerLayer);

            // Initialize the photonView reference
            photonView = GetComponent<PhotonView>();
            if (photonView == null) Debug.LogError("PhotonView is null in PlayerMove.Awake()");
            else
            {
                photonView.name = PhotonNetwork.LocalPlayer.NickName;
                Debug.Log($"PhotonView name: {photonView.name}, PhotonView ID: {photonView.ViewID}, PhotonView ownership: {(photonView.IsMine ? "true" : "false")}");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            if (!photonView.IsMine)
                Destroy(rb);
            if (photonView.IsMine)
            {
                StartCoroutine(NotifyGameManagerWhenReady());
                this.manipulator = Instantiate(manipulator, new Vector3(0, 0), new Quaternion(0, 0, 0, 0));
                this.manipulator.player = gameObject;
            }
        }

        private IEnumerator NotifyGameManagerWhenReady()
        {
            yield return new WaitForSeconds(0.1f); // Small delay to ensure initialization is complete

            if (photonView.IsMine)
            {
                var properties = new ExitGames.Client.Photon.Hashtable { { "IsReady", true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                //Restart();
                if (alive)
                {
                    //Hurt();
                    Die();
                    //Attack();
                    JumpF();
                    //KickBoard();
                    Run();

                }
            }
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
            //Debug.Log($"[RPC_SetUsername] RPC called with: {usernameString} for {photonView.Owner.NickName} - {photonView.ViewID}");
            if (Username != null)
            {
                Username.text = usernameString;
                Username.color = photonView.IsMine ? Color.yellow : Color.white;
            }
            else
                Debug.LogError("[RPC_SetUsername] Username TextMeshProUGUI component is not assigned!");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);
        }

        /*void KickBoard()
        {
            if (Input.GetKeyDown(KeyCode.Alpha4) && isKickboard)
            {
                isKickboard = false;
                anim.SetBool("isKickBoard", false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && !isKickboard )
            {
                isKickboard = true;
                anim.SetBool("isKickBoard", true);
            }

        }*/

        void Run()
        {
            /*if (!isKickboard)
            {*/
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);


            if (Input.GetKey(MoveLeft) && !Input.GetKey(MoveRight))
            {
                direction = -1;
                moveVelocity = Vector3.left;

                transform.localScale = new Vector3(direction * scale_x, scale_y, scale_z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            if (Input.GetKey(MoveRight) && !Input.GetKey(MoveLeft))
            {
                direction = 1;
                moveVelocity = Vector3.right;

                transform.localScale = new Vector3(direction * scale_x, scale_y, scale_z);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            transform.position += moveVelocity * movePower * Time.deltaTime;

            //}
            /*if (isKickboard)
            {
                Vector3 moveVelocity = Vector3.zero;
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    direction = -1;
                    moveVelocity = Vector3.left;

                    transform.localScale = new Vector3(direction, 1, 1);
                }
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    direction = 1;
                    moveVelocity = Vector3.right;

                    transform.localScale = new Vector3(direction, 1, 1);
                }
                transform.position += moveVelocity * KickBoardMovePower * Time.deltaTime;
            }*/
        }
        void JumpF()
        {
            if ((Input.GetKeyDown(Jump))
            && !anim.GetBool("isJump"))
            {
                isJumping = true;
                anim.SetBool("isJump", true);
            }
            if (!isJumping)
            {
                return;
            }

            rb.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }
        /*void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetTrigger("attack");
            }
        }*/
        /*void Hurt()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                anim.SetTrigger("hurt");
                if (direction == 1)
                    rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
                else
                    rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
            }
        }*/
        void Die()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //isKickboard = false;
                //anim.SetBool("isKickBoard", false);
                anim.SetTrigger("die");
                alive = false;
            }
        }
        /*void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                //isKickboard = false;
                anim.SetBool("isKickBoard", false);
                anim.SetTrigger("idle");
                alive = true;
            }
        }*/

        // Synchronize state with other players
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(alive);
                //stream.SendNext(isKickboard);
                stream.SendNext(direction);
                stream.SendNext(transform.position);
            }
            else
            {
                alive = (bool)stream.ReceiveNext();
                //isKickboard = (bool)stream.ReceiveNext();
                direction = (int)stream.ReceiveNext();
                transform.position = (Vector3)stream.ReceiveNext();
            }
        }
    }

}