using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRb;

    public float Speed = 10f;

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        bulletRb.velocity = transform.forward * Speed;
        //DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject, 2f);
    }
}
