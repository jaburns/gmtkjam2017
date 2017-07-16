using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [Serializable]
    class MovementParams {
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
    [SerializeField] float _bloodPerBullet;

    bool _inBlood = false;
    bool _inBloodMode = false;
    BloodController _bloodController;
    Rigidbody _rb;
    int _grounded;
    int _livingBullets = 0;
    bool _pressedSpace = false;

    bool _clickDrinking = false;
    bool _clickShooting = false;
    bool _clickDone = false;

    public float PersonalBlood { get; private set; }
    public float LivingBulletBlood { get { return _livingBullets * _bloodPerBullet; } }

    void Start()
    {
        _bloodController = FindObjectOfType<BloodController>();
        _rb = GetComponent<Rigidbody>();
        PersonalBlood = 1.0f;
    }

    void FixedUpdate()
    {
        var _curMovement = _inBloodMode ? _bloodMovement : _airMovement;

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
            if (!_clickDrinking && !_clickShooting) {
                if (_inBloodMode) _clickDrinking = true;
                else _clickShooting = true;
            }
        } else {
            _clickDone = false;
            _clickDrinking = false;
            _clickShooting = false;
        }

        if (!_clickDone) {
            if (_clickDrinking) {
                if (_inBlood) drink();
                else if (!_inBloodMode) _clickDone = true;
            }
            else if (_clickShooting) {
                if (!_inBloodMode) shoot();
                else _clickDone = true;
            }
        }
    }

    void Update()
    {
        _bloodMeter.transform.localScale += Vector3.up * (PersonalBlood - _bloodMeter.transform.localScale.y) / 2.0f;
    }

    void drink()
    {
        if (PersonalBlood >= 1.0f) return;
        PersonalBlood += _bloodDrinkSpeed;
        transform.position += Vector3.down * _bloodController.HeightPerBloodAmount * _bloodDrinkSpeed;
        if (PersonalBlood > 1.0f) PersonalBlood = 1.0f;
    }

    public void NotifyBulletDied()
    {
        _livingBullets--;
    }

    void shoot()
    {
        if (PersonalBlood < 0.1f) return;

        var playerPlane = new Plane(Vector3.up, _rb.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (! playerPlane.Raycast(ray, out rayDistance)) return;
        var aimPos = ray.GetPoint(rayDistance);
        var aimVec = (aimPos - _rb.position).normalized;

        var bullet = Instantiate(_bulletPrefab, _rb.position, Quaternion.identity) as GameObject;
        var bc = bullet.GetComponent<BulletController>();
        bc.Init(this, aimVec);
        _livingBullets++;
        PersonalBlood -= _bloodPerBullet;
    }

    void OnEnterBlood()
    {
        _inBlood = true;
    }

    void OnExitBlood()
    {
        StartCoroutine(bloodFalseLater());
    }

    IEnumerator bloodFalseLater()
    {
        yield return new WaitForEndOfFrame();
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
                _inBloodMode = _inBlood;
            }
        }
    }
}
