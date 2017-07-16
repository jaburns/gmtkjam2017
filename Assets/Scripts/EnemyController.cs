using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{
    public float MoveForce;
    public float DragCoeff;
    public float health;

    [SerializeField] float _bloodAmount;
    [SerializeField] GameObject _deathParticles;
    
    Rigidbody _rb;
    PlayerController _player;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var dxz = (_player.transform.position - transform.position);
        dxz.y = 0;
        _rb.AddForce(dxz * MoveForce);
        var v_xz = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(DragCoeff * -v_xz);
    }

    void OnGetShot()
    {
        health -= 1.0f;
        if (health < 0.0f) {
            Instantiate(_deathParticles, transform.position, Quaternion.identity);
            FindObjectOfType<BloodController>().IncreaseAmbientBloodLevel(_bloodAmount);
            Destroy(gameObject);
        }
    }
}
