using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    [SerializeField] float _speed; 
    [SerializeField] GameObject _deathParticles; 
    PlayerController _pc;
    float _t;

    public void Init(PlayerController pc, Vector3 direction)
    {
        _pc = pc;
        _t = Time.time;
        GetComponent<Rigidbody>().velocity = direction * _speed;
    }


    void FixedUpdate()
    {
        if (Time.time > _t + 2.0f) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision c)
    {
        foreach (var o in c.contacts) {
            o.otherCollider.SendMessage(_pc == null ? "OnGetEnemyShot" : "OnGetShot", SendMessageOptions.DontRequireReceiver);
        }

        Instantiate(_deathParticles, transform.position, Quaternion.FromToRotation(Vector3.forward, c.contacts[0].normal));
        Destroy(gameObject);
    }
}