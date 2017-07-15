using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{
    void OnGetShot()
    {
        Destroy(gameObject);
    }
}
