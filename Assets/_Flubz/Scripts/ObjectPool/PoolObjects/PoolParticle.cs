using System.Collections;
using System.Collections.Generic;
using SollaraGames.Managers;
using SollaraGames.ObjectPooling;
using UnityEngine;

namespace SollaraGames.ObjectPooling.PoolObjects
{
	public class PoolParticle : PoolObject
	{
		[SerializeField] ParticleSystem _ps;
		[SerializeField] float _additionalTime;
		float _particleDuration;

		private void Awake ()
		{
			_particleDuration = _ps.main.duration + _additionalTime;
		}

		private void OnEnable ()
		{
			_ps.Play ();
			StartCoroutine (LifeTime ());
		}

		IEnumerator LifeTime ()
		{
			yield return new WaitForSeconds (_particleDuration);
			PoolManager._instance.ReturnObjectToPool (this, true);
		}
	}
}