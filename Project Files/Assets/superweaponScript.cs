using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace Superweapon
{
    public class superweaponScript : MonoBehaviourPunCallbacks
    {
        public int killCoordinateX;
        public float projectileSpeed;
        public Manipulator manipulator;
        public bool? direction = null;
        // Start is called before the first frame update
        void Start()
        {
            Debug.LogWarning("SWShot was created");
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
            {
                if (this.direction != null)
                {
                    if ((bool)this.direction)
                        transform.position = transform.position + Vector3.left * projectileSpeed * Time.deltaTime;
                    else
                        transform.position = transform.position + Vector3.right * projectileSpeed * Time.deltaTime;
                }

            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Time Manipulable"))
            {
                this.manipulator.destroyObject(collision.gameObject);
                Destroy(gameObject);
            }
        }

        public void setDirection(bool dir)
        {
            if (this.direction == null) //only sets the direction if it hadn't been set before
            {
                this.direction = dir;
                photonView.RPC("RPC_Direction", RpcTarget.Others, dir);
            }
        }

        [PunRPC]
        public void RPC_Direction(bool dir)
        {
            this.direction = dir;
        }
    }
}
