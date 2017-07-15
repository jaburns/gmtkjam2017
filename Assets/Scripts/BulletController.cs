using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    [SerializeField] float _speed; 
    [SerializeField] GameObject _deathParticles; 

    PlayerController _pc;

    public void Init(PlayerController pc, Vector3 direction)
    {
        _pc = pc;
        GetComponent<Rigidbody>().velocity = direction * _speed;
    }

    void OnCollisionEnter(Collision c)
    {
        foreach (var o in c.contacts) {
            o.otherCollider.SendMessage("OnGetShot", SendMessageOptions.DontRequireReceiver);
        }

        Instantiate(_deathParticles, transform.position, Quaternion.FromToRotation(Vector3.forward, c.contacts[0].normal));
        Destroy(gameObject);
        _pc.NotifyBulletDied();
    }
}