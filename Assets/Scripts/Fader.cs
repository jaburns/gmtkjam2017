using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour 
{
	Material _fadeMat;
	public Color _mainColor;
	public float fadeRate = 0.9f;

	float fade;

	void Awake()
	{
		fade = 1;
		_fadeMat = transform.Find("Quad").GetComponent<MeshRenderer>().sharedMaterial;
	}

	void Update()
	{
		fade *= fadeRate;
		_mainColor.a = fade;
		_fadeMat.SetColor("_Color", _mainColor);

		if (fade < 0.003f) {
			Destroy(gameObject);
		}
	}
}
