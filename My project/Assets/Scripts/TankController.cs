using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankController : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                Move();

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Fire();
                }
            }
        }
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float rotate = -Input.GetAxis("Vertical") * rotateSpeed * Time.deltaTime;

        transform.Translate(0, move, 0);
        transform.Rotate(0, 0, rotate);
    }

    void Fire()
    {
        PhotonNetwork.Instantiate("Prefabs/bullet", firePoint.position, firePoint.rotation);
    }

}
