using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    [SerializeField] float _speed; 
    [SerializeField] GameObject _deathParticles; 
    [SerializeField] float spread; 
    [SerializeField] float scaleSpread;
    PlayerController _pc;
    float _t = -1f;

    public void Init(PlayerController pc, Vector3 direction)
    {
        var randy = UnityEngine.Random.onUnitSphere;
        randy.y = 0;
        randy = randy.normalized;
        direction = direction.normalized;
        direction += randy * spread;

        _pc = pc;
        _t = Time.time;
        GetComponent<Rigidbody>().velocity = direction * _speed;

        transform.localScale *= UnityEngine.Random.value * scaleSpread + (1f - scaleSpread);
    }

    void FixedUpdate()
    {
        if (_t > 0.0f && Time.time > _t + 5.0f) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision c)
    {
        foreach (var o in c.contacts) {
            o.otherCollider.SendMessage(_pc == null ? "OnGetEnemyShot" : "OnGetShot", SendMessageOptions.DontRequireReceiver);
        }

        if (_t >= 0.0f) {
            Instantiate(_deathParticles, transform.position, Quaternion.FromToRotation(Vector3.forward, c.contacts[0].normal));
            Destroy(gameObject);
        }
    }
}