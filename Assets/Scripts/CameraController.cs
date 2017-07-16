using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] float _lag;
    [SerializeField] float _rockRadius;
    [SerializeField] float _rockFrequency;

    PlayerController _blood;
    Rigidbody _rb;
    float _bloodDeltaY;
    float _baseX;
    float _baseZ;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _blood = FindObjectOfType<PlayerController>();
        _bloodDeltaY = transform.position.y - _blood.transform.position.y;
        _baseX = transform.position.x;
        _baseZ = transform.position.z;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(new Vector3(
            _baseX + _rockRadius * Mathf.Sin(_rockFrequency * Time.time),
            _rb.position.y + (_blood.transform.position.y + _bloodDeltaY - _rb.position.y) / _lag,
            _baseZ + _rockRadius * (-1f + Mathf.Cos(_rockFrequency * Time.time))
        ));
    }
}
