using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    [SerializeField] float _speed; 

    Rigidbody _rb;

    public void Init(Vector3 direction)
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = direction * _speed;
        StartCoroutine(killIt());
    }

    IEnumerator killIt()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}