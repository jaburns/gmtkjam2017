using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
    [SerializeField] float _bulletHeight;
    [SerializeField] GameObject _moveThingA;
    [SerializeField] GameObject _moveThingB;
    [SerializeField] GameObject _splorshPrefab;

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
    bool _playLoopShoot = false;
    bool _playLoopDrink = false;

    Vector3 _aimVec;
    Vector3 _forceVec;
    Animator _anim;

    CameraController _cam;
    Collider _curGround = null;
    GameObject _splash;

    public float PersonalBlood { get; private set; }
    public float LivingBulletBlood { get { return FindObjectsOfType<HeroBullet>().Length * _bloodPerBullet; } }

    void Start()
    {
        _bloodController = FindObjectOfType<BloodController>();
        _rb = GetComponent<Rigidbody>();
        _cam = FindObjectOfType<CameraController>();
        PersonalBlood = 1.0f;

        _splash = transform.Find("SplashingAround").gameObject;
        _anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
        var _curMovement = _inBloodMode ? _bloodMovement : _airMovement;

        if(_rb.IsSleeping()) _rb.WakeUp();

        if (_grounded > 0) {
            if (Input.GetKey(KeyCode.Space) && !_pressedSpace) {
                _rb.AddForce(_curMovement.JumpImpulse * Vector3.up, ForceMode.Impulse);
                _grounded = 0;
                Audio.Play("Jump");
            }
            _grounded--;
            _splash.SetActive(_inBloodMode);
        } else {
            _curGround = null;
            _splash.SetActive(false);
        }
        _pressedSpace = Input.GetKey(KeyCode.Space);

        var newForceVec = Vector3.zero;
        var xxx = false;
        if (Input.GetKey(KeyCode.W)) { xxx = true; sendWalkMessage(); newForceVec += (_curMovement.MoveForce * Vector3.forward); }
        if (Input.GetKey(KeyCode.S)) { xxx = true; sendWalkMessage(); newForceVec += (_curMovement.MoveForce * Vector3.back); }
        if (Input.GetKey(KeyCode.A)) { xxx = true; sendWalkMessage(); newForceVec += (_curMovement.MoveForce * Vector3.left); } 
        if (Input.GetKey(KeyCode.D)) { xxx = true; sendWalkMessage(); newForceVec += (_curMovement.MoveForce * Vector3.right); }

        _rb.AddForce(newForceVec);
        if (xxx) _forceVec = newForceVec;

        _anim.SetBool("isWalking", xxx);
        _anim.SetBool("isGrounded", _grounded > 0);
        _anim.SetBool("isIdle", !xxx);

        var v_xz = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        _rb.AddForce(_curMovement.DragCoeff * -v_xz);

        if (Input.GetMouseButton(0)) {
            if (!_clickDrinking && !_clickShooting) {
                if (_inBlood) _clickDrinking = true;
                else _clickShooting = true;
            }
        } else {
            _clickDone = false;
            _clickDrinking = false;
            _clickShooting = false;
        }

        _cam.LowLevelShake = false;

        _playLoopShoot = false;
        _playLoopDrink = false;

        if (!_clickDone) {
            if (_clickDrinking) {
                if (_inBlood) drink();
                else if (!_inBlood) _clickDone = true;
            }
            else if (_clickShooting) {
                if (!_inBlood) shoot();
                else _clickDone = true;
            }
        }

        Audio.SetLoopPlaying("ShootBlood", _playLoopShoot);
        Audio.SetLoopPlaying("DrinkBlood", _playLoopDrink);
    }

    void sendWalkMessage()
    {
        if (_curGround != null) {
            _curGround.SendMessage("OnPlayerWalk", SendMessageOptions.DontRequireReceiver);
        }
    }

    void Update()
    {
        var playerPlane = new Plane(Vector3.up, transform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (! playerPlane.Raycast(ray, out rayDistance)) return;
        var aimPos = ray.GetPoint(rayDistance);
        _aimVec = (aimPos - transform.position).normalized;
        _aimVec.y = 0;

        _bloodMeter.transform.localScale += Vector3.up * (PersonalBlood - _bloodMeter.transform.localScale.y) / 2.0f;
		_moveThingA.transform.rotation = Quaternion.Euler(0.0f, -90.0f + Mathf.Rad2Deg * Mathf.Atan2(-_forceVec.z, _forceVec.x), 0.0f);
        _moveThingB.transform.rotation = Quaternion.Euler(-90.0f, -270.0f + Mathf.Rad2Deg * Mathf.Atan2(-_aimVec.z, _aimVec.x), 0.0f);
      
    }

    void drink()
    {
        if (PersonalBlood >= 1.0f) return;
        _playLoopDrink = true;
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
        if (PersonalBlood < 0.001f) return;

        _cam.LowLevelShake = true;
        _playLoopShoot = true;

        var bullet = Instantiate(_bulletPrefab, _rb.position + Vector3.up * _bulletHeight, Quaternion.identity) as GameObject;
        var bc = bullet.GetComponent<BulletController>();
        bc.Init(this, _aimVec);
        _livingBullets++;
        PersonalBlood -= _bloodPerBullet;
        if (PersonalBlood < 0.0f) PersonalBlood = 0.0f;
    }

    void OnGetEnemyShot()
    {
        Audio.Play("GetHit");
        Instantiate(_splorshPrefab, transform.position, Quaternion.identity);
        (new GameObject()).AddComponent<Restarter>();
        Destroy(gameObject);
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
                _curGround = n.otherCollider;
                _grounded = _groundTime;
                _inBloodMode = _inBlood;
            }
        }
    }
}
