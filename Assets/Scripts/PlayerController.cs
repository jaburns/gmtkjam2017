using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [Serializable]
    struct MovementParams {
        public float MoveForce;
        public float DragCoeff;
        public float JumpImpulse;
    }

    [SerializeField] MovementParams _bloodMovement;
    [SerializeField] MovementParams _airMovement;
    [SerializeField] int _groundTime;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _bloodMeter;
    [SerializeField] float _bloodDrinkSpeed;

    bool _inBlood = false;
    BloodController _bloodController;
    MovementParams _curMovement;
    Rigidbody _rb;
    int _grounded;
    bool _pressedSpace = false;

    float _personalBlood = 1.0f;

    void Start()
    {
        _bloodController = FindObjectOfType<BloodController>();
        _rb = GetComponent<Rigidbody>();
        _curMovement = _airMovement;
    }

    void FixedUpdate()
    {
        if(_rb.IsSleeping()) _rb.WakeUp();

        if (_grounded > 0) {
            if (Input.GetKey(KeyCode.Space) && !_pressedSpace) {
                _rb.AddForce(_curMovement.JumpImpulse * Vector3.up, ForceMode.Impulse);
                _grounded = 0;
            }
            _grounded--;
        }
        _pressedSpace = Input.GetKey(KeyCode.Space);

        if (Input.GetKey(KeyCode.W)) _rb.AddForce(_curMovement.MoveForce * Vector3.forward);
        if (Input.GetKey(KeyCode.S)) _rb.AddForce(_curMovement.MoveForce * Vector3.back);
        if (Input.GetKey(KeyCode.A)) _rb.AddForce(_curMovement.MoveForce * Vector3.left);
        if (Input.GetKey(KeyCode.D)) _rb.AddForce(_curMovement.MoveForce * Vector3.right);

        var v_xz = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        _rb.AddForce(_curMovement.DragCoeff * -v_xz);

        if (Input.GetMouseButton(0)) {
            if (_inBlood) {
                drink();
            } else {
                shoot();
            }
        }
    }

    void Update()
    {
        _bloodMeter.transform.localScale += Vector3.up * (_personalBlood - _bloodMeter.transform.localScale.y) / 2.0f;
    }

    void drink()
    {
        if (_personalBlood >= 1.0f) return;

        var oldBlood = _personalBlood;
        _personalBlood += _bloodDrinkSpeed;
        if (_personalBlood > 1.0f) _personalBlood = 1.0f;
        _bloodController.ChangeHeight(oldBlood - _personalBlood);
    }

    void shoot()
    {
        if (_personalBlood < 0.1f) return;

        var playerPlane = new Plane(Vector3.up, _rb.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (! playerPlane.Raycast(ray, out rayDistance)) return;
        var aimPos = ray.GetPoint(rayDistance);
        var aimVec = (aimPos - _rb.position).normalized;

        var bullet = Instantiate(_bulletPrefab, _rb.position, Quaternion.identity) as GameObject;
        var bc = bullet.GetComponent<BulletController>();
        bc.Init(_bloodController, aimVec);
        _personalBlood -= bc.BloodAmount;
    }

    void OnEnterBlood()
    {
        _curMovement = _bloodMovement;
        _inBlood = true;
    }

    void OnExitBlood()
    {
        _curMovement = _airMovement;
        _inBlood = false;
    }

    void OnCollisionEnter(Collision c)
    {
        foreach(var n in c.contacts) {
            if (n.normal.y > 0.5f) {
                _rb.velocity = Vector3.zero;
            }
        }
    }

    void OnCollisionStay(Collision c)
    {
        foreach(var n in c.contacts) {
            if (n.normal.y > 0.5f) {
                _grounded = _groundTime;
            }
        }
    }
}
