using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController : MonoBehaviour 
{
    [SerializeField] float _heightPerBloodAmount = 1.0f;

    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ChangeHeight(float deltaBlood)
    {
        _rb.MovePosition(_rb.position + Vector3.up * _heightPerBloodAmount * deltaBlood);
    }
}

