using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField] float _moveForce = 10.0f;
    [SerializeField] float _dragCoeff = 10.0f;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) _rb.AddForce(_moveForce * Vector3.forward);
        if (Input.GetKey(KeyCode.S)) _rb.AddForce(_moveForce * Vector3.back);
        if (Input.GetKey(KeyCode.A)) _rb.AddForce(_moveForce * Vector3.left);
        if (Input.GetKey(KeyCode.D)) _rb.AddForce(_moveForce * Vector3.right);

        var v_xz = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        _rb.AddForce(_dragCoeff * -v_xz);
    }
}
