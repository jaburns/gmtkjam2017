using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController : MonoBehaviour 
{
    public float HeightPerBloodAmount = 1.0f;
    public float Lag = 3.0f;

    PlayerController _player;
    Rigidbody _rb;
    float _baseY;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        _baseY = _rb.position.y + HeightPerBloodAmount;
    }

    void FixedUpdate()
    {
        var targetY = _baseY - HeightPerBloodAmount * (_player.PersonalBlood + _player.LivingBulletBlood);
        _rb.MovePosition(new Vector3(
            _rb.position.x, 
            _rb.position.y + (targetY - _rb.position.y) / Lag,
            _rb.position.z
        ));
    }
    
    public void IncreaseAmbientBloodLevel(float delta)
    {
        _baseY += delta;
        Debug.Log(_baseY);
    }
}

