using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour 
{
	MeshRenderer rendy;
	Material _fadeMat;
	public Color _mainColor;
	public float fadeRate = 0.9f;
	public Color _fadeToBlackColor;
	public float _fadeToBlackRate;
	bool _fadingToBlack;
	float _fadingToBlackT;

	float fade;

	void Awake()
	{
		fade = 1;
		rendy = transform.Find("Quad").GetComponent<MeshRenderer>();
		_fadeMat = rendy.sharedMaterial;
	}

	void Update()
	{
		if (_fadingToBlack) {
			_fadingToBlackT += _fadeToBlackRate;
			if (_fadingToBlackT > 1f) {
				SceneManager.LoadScene("Endor");
			}
			_fadeToBlackColor.a = _fadingToBlackT;
			_fadeMat.SetColor("_Color", _fadeToBlackColor);
			return;
		}

		fade *= fadeRate;
		_mainColor.a = fade;
		_fadeMat.SetColor("_Color", _mainColor);

		if (fade < 0.003f) {
			rendy.gameObject.SetActive(false);
		}
	}

	public void FadeToBlack()
	{
		rendy.gameObject.SetActive(true);
		_fadingToBlack = true;
		_fadingToBlackT = 0f;
	}
}
