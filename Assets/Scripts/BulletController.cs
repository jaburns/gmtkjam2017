using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    [SerializeField] float _speed; 

    PlayerController _pc;

    public void Init(PlayerController pc, Vector3 direction)
    {
        _pc = pc;
        GetComponent<Rigidbody>().velocity = direction * _speed;
    }

    void OnCollisionEnter(Collision c)
    {
        Destroy(gameObject);
        _pc.NotifyBulletDied();
    }
}