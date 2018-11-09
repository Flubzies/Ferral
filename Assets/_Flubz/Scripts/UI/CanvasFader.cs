using System;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof (CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
	[SerializeField] CanvasGroup _canvasGroup;
	[SerializeField] float _fadeDuration = 0.5f;
	[SerializeField] AnimationCurve _curve = AnimationCurve.EaseInOut (0, 0, 1, 1);

	[SerializeField] bool _pauseTime = true;

	public Action OnFadeInComplete;
	public Action OnFadeOutComplete;

	bool _isFading;

	public void FadeCanvasIn ()
	{
		if (_isFading) return;
		StartCoroutine (FadeIn ());
	}

	public void FadeCanvasOut ()
	{
		if (_isFading) return;
		StartCoroutine (FadeOut ());
	}

	IEnumerator FadeIn ()
	{
		float temp = 0.0f;
		float alpha = 0.0f;

		_canvasGroup.blocksRaycasts = true;
		_canvasGroup.interactable = true;
		_isFading = true;
		if (_pauseTime) Time.timeScale = 0;

		while (temp < _fadeDuration)
		{
			if (_pauseTime) temp += Time.unscaledDeltaTime;
			else temp += Time.deltaTime;

			alpha = temp.Remap (0, _fadeDuration, 0, 1);
			_canvasGroup.alpha = _curve.Evaluate (alpha);

			// yield return new WaitForSecondsRealtime (0);
			yield return null;
		}

		_isFading = false;
		_canvasGroup.alpha = 1.0f;

		if (OnFadeInComplete != null) OnFadeInComplete.Invoke ();
	}

	IEnumerator FadeOut ()
	{
		float temp = _fadeDuration;
		float alpha = 1.0f;
		_isFading = true;

		while (temp > 0)
		{
			if (_pauseTime) temp -= Time.unscaledDeltaTime;
			else temp -= Time.deltaTime;

			alpha = temp.Remap (0, _fadeDuration, 0, 1);
			_canvasGroup.alpha = _curve.Evaluate (alpha);

			yield return null;
		}

		if (_pauseTime) Time.timeScale = 1;
		_isFading = false;
		_canvasGroup.alpha = 0.0f;
		_canvasGroup.blocksRaycasts = false;
		_canvasGroup.interactable = false;

		if (OnFadeOutComplete != null) OnFadeOutComplete.Invoke ();
	}
}