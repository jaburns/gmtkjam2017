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
    [SerializeField] float _randomness = 2.0f;

    int _dontShootTimer = 0;
    int _shotsLeft = 0;
    
    Rigidbody _rb;
    PlayerController _player;
    Vector3 _randWalk;

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var dxz = (_player.transform.position - transform.position).normalized;

        dxz.y = 0;
        _rb.AddForce((dxz + _randomness*_randWalk).normalized * MoveForce);
        var v_xz = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(DragCoeff * -v_xz);

        if (_dontShootTimer > 0) {
            _dontShootTimer--;
        } else if (Random.value < _shootFreq) {
            _shotsLeft = 3;
            _randWalk = (new Vector3(Random.value - 0.5f, 0.0f, Random.value - 0.5f)).normalized;
        }

        if (_shotsLeft > 0) {
            _shotsLeft--;
            var bullet = Instantiate(_bullet, _rb.position + Vector3.up * _bulletHeight, Quaternion.identity) as GameObject;
            var bc = bullet.GetComponent<BulletController>();
            bc.Init(null, dxz);
            if (_shotsLeft <= 0) {
                _dontShootTimer = 60;
            }
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
