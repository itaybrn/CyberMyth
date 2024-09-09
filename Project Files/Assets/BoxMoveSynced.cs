using Photon.Pun;
using UnityEngine;

public class BoxMoveSynced : HasID, IPunObservable
{
    public float moveSpd;
    private Vector2 networkedPosition;
    private Vector2 networkedVelocity;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            rb2D.velocity = Vector2.left * moveSpd;
        }
        else
        {
            rb2D.position = networkedPosition;
            rb2D.velocity = networkedVelocity;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Sending
        {
            stream.SendNext(rb2D.position);
            stream.SendNext(rb2D.velocity);
        }
        else //Receiving
        {
            networkedPosition = (Vector2)stream.ReceiveNext();
            networkedVelocity = (Vector2)stream.ReceiveNext();
        }
    }
}
