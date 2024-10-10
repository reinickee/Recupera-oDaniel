using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public int damage = 20;  // Quantidade de dano que a bala causa

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        Destroy(gameObject, lifeTime);  // Destruir após determinado tempo
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto com o qual colidimos implementa a interface IDamageable
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            // Aplica dano ao objeto
            damageable.TakeDamage(damage);
        }

        // Destroi o projétil ao colidir
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}