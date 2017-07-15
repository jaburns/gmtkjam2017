using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] float _lag;
    BloodController _blood;
    Rigidbody _rb;
    float _bloodDeltaY;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _blood = FindObjectOfType<BloodController>();
        _bloodDeltaY = transform.position.y - _blood.transform.position.y;
    }

    void FixedUpdate()
    {
        _rb.MovePosition(new Vector3(
            _rb.position.x, 
            _rb.position.y + (_blood.transform.position.y + _bloodDeltaY - _rb.position.y) / _lag,
            _rb.position.z
        ));
    }
}
