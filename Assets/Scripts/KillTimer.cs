using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour 
{
	[SerializeField] float _time;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(_time);
		Destroy(gameObject);
	}
}
