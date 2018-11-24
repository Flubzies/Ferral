using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestructParticle : MonoBehaviour
{
	[SerializeField] ParticleSystem _ps;
	[SerializeField] float _additionalTime;
	float _particleDuration;

	private void Awake ()
	{
		_particleDuration = _ps.main.duration + _additionalTime;
		_ps.Play ();
		StartCoroutine (LifeTime ());
	}

	IEnumerator LifeTime ()
	{
		yield return new WaitForSeconds (_particleDuration);
		Destroy (gameObject);
	}
}