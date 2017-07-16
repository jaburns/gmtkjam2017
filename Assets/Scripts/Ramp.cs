using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour 
{
	[SerializeField] PhysicMaterial _defaultMat;

	PhysicMaterial _originalMat;
	Collider _col;
	int _framesSinceWalk;
	bool _guyWalking;

	void Start () {
		_col = GetComponent<Collider>();
		_originalMat = _col.sharedMaterial;
	}

	void FixedUpdate() 
	{
		if (_framesSinceWalk > 0) {
			if (!_guyWalking) {
				startWalk();
				_guyWalking = true;
			}
			_framesSinceWalk--;
			if (_framesSinceWalk == 0) {
				_guyWalking = false;
				endWalk();
			}
		}
	}

	void startWalk()
	{
		_col.sharedMaterial = _defaultMat;
	}

	void endWalk()
	{
		_col.sharedMaterial = _originalMat;
	}

	void OnPlayerWalk()
	{
		_framesSinceWalk = 3;
	}
}
