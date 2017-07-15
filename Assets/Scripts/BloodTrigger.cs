using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodTrigger : MonoBehaviour 
{
    void OnTriggerEnter(Collider x)
    {
        x.SendMessage("OnEnterBlood", SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider x)
    {
        x.SendMessage("OnExitBlood", SendMessageOptions.DontRequireReceiver);
    }
}
