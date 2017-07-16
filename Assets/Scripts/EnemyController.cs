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
    [SerializeField] GameObject _bullet;
    [SerializeField] float _bulletHeight;
    [SerializeField] float _shootFreq;
    
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

        if (Random.value < _shootFreq) {
            var bullet = Instantiate(_bullet, _rb.position + Vector3.up * _bulletHeight, Quaternion.identity) as GameObject;
            var bc = bullet.GetComponent<BulletController>();
            bc.Init(null, dxz);
        }
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
