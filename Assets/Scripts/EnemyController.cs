using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{
    [SerializeField] float _bloodAmount;
    [SerializeField] GameObject _deathParticles;

    void OnGetShot()
    {
        Instantiate(_deathParticles, transform.position, Quaternion.identity);
        FindObjectOfType<BloodController>().IncreaseAmbientBloodLevel(_bloodAmount);
        Destroy(gameObject);
    }
}
