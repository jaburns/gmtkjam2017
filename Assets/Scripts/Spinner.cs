using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour 
{
	public float speed = 1.0f;

	void Update()
	{
		transform.rotation = Quaternion.Euler(0, 0, Time.time * speed);
	}
}
