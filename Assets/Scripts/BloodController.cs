using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController : MonoBehaviour 
{
    [SerializeField] float _heightPerBloodAmount = 1.0f;

    PlayerController _player;
    Rigidbody _rb;
    float _baseY;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        _baseY = _rb.position.y + _heightPerBloodAmount;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(new Vector3(
            _rb.position.x, 
            _baseY - _heightPerBloodAmount * (_player.PersonalBlood + _player.LivingBulletBlood),
            _rb.position.z
        ));
    }
}

