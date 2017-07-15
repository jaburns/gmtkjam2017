using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    public float BloodAmount = 0.01f;
    [SerializeField] float _speed; 

    Rigidbody _rb;
    BloodController _bc;

    public void Init(BloodController bc, Vector3 direction)
    {
        _bc = bc;
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = direction * _speed;
    }

    void OnCollisionEnter(Collision c)
    {
        _bc.ChangeHeight(BloodAmount);
        Destroy(gameObject);
    }
}