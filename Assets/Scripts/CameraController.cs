using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] float _lag;
    [SerializeField] float _rockRadius;
    [SerializeField] float _rockFrequency;
    [SerializeField] float _shakeAttack = 0.1f;
    [SerializeField] float _shakeDecay = 0.95f;
    [SerializeField] float _lowLevelShake = 0.01f;

    PlayerController _blood;
    Rigidbody _rb;
    float _bloodDeltaY;
    float _baseX;
    float _baseZ;

    Vector3 _shakeVector;
    float _shakeWeight;

    [HideInInspector]
    public bool LowLevelShake = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _blood = FindObjectOfType<PlayerController>();
        _bloodDeltaY = transform.position.y - _blood.transform.position.y;
        _baseX = transform.position.x;
        _baseZ = transform.position.z;
        _shakeWeight = 0;
    }

    void FixedUpdate()
    {
        if (_blood == null) return;
        
        if (LowLevelShake && _shakeWeight < _lowLevelShake) {
            _shakeWeight = _lowLevelShake;
        }
        _shakeVector = (new Vector3(Random.value-0.5f, 0, Random.value-0.5f)).normalized;
        _shakeVector *= _shakeWeight;
        _shakeWeight *= _shakeDecay;

        _rb.MovePosition(_shakeVector + new Vector3(
            _baseX + _rockRadius * Mathf.Sin(_rockFrequency * Time.time),
            _rb.position.y + (_blood.transform.position.y + _bloodDeltaY - _rb.position.y) / _lag,
            _baseZ + _rockRadius * (-1f + Mathf.Cos(_rockFrequency * Time.time))
        ));
    }

    public void Shake()
    {
         _shakeWeight = _shakeAttack;
    }
}
