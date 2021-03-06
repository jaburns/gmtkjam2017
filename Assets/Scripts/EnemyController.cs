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
    [SerializeField] bool _boss = false;

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
        if (_player == null) return;

        var ndifs = _player.transform.position - transform.position;
        if (Mathf.Abs(ndifs.y) > 5.0f) return;

        var dxz = ndifs.normalized;
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

    void Update()
    {
        if (_player == null) return; 
        var dxz = (_player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Euler(0, 180 + Mathf.Rad2Deg * Mathf.Atan2(dxz.z, -dxz.x), 0);
    }

    bool killed = false;
    void OnGetShot()
    {
        if (killed) return;
        health -= 1.0f;
        if (health < 0.0f) {
            killed = true;
			Instantiate(_deathParticles, transform.position, Quaternion.Euler(-90,0,0));
            FindObjectOfType<BloodController>().IncreaseAmbientBloodLevel(_bloodAmount);
            FindObjectOfType<CameraController>().Shake();
            Audio.Play("Death");
            Destroy(gameObject);
            if (_boss) {
                PlayerController.INVULN = true;
                FindObjectOfType<Fader>().FadeToBlack();
            }
        }
    }
}
