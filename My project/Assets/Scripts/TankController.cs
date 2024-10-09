using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TankController : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
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

    void Move()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = -Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Translate(0, move, 0);
        transform.Rotate(0, 0, rotate);
    }

    void Fire()
    {
        PhotonNetwork.Instantiate("Prefabs/Bullet", firePoint.position, firePoint.rotation);
    }

}
