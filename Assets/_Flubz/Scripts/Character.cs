using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	[SerializeField] int _damage;
	[SerializeField] float _getRandomPositionRate;
	[SerializeField] float _movementUpdateRate;
	[SerializeField] NavMeshAgent _agent;
	[SerializeField] float _spawnWaitTime = 2.0f;
	[SerializeField] float _spawnDuration = 0.5f;
	[SerializeField] Vector3 _finalScale;
	[SerializeField] float _randomPositionRadius = 10.0f;

	Vector3 _target;

	private void Start ()
	{
		_agent.enabled = false;
		StartCoroutine (SpawnWaitTime ());
	}

	IEnumerator SpawnWaitTime ()
	{
		transform.localScale = Vector3.zero;
		transform.DOScale (_finalScale, _spawnDuration);
		yield return new WaitForSeconds (_spawnWaitTime);
		_agent.enabled = true;
		if (_agent.isOnNavMesh)
		{
			_agent.Warp (transform.position);
		}
		else OnDeath ();
		StartCoroutine (MoveTowardsTarget ());
	}

	IEnumerator MoveTowardsTarget ()
	{
		if (_target != null)
		{
			if (_agent.isOnNavMesh)
			{
				_agent.SetDestination (_target);
			}
			else OnDeath ();
		}
		else
		{
			StartCoroutine (GetRandomPosition ());
		}

		yield return new WaitForSeconds (_movementUpdateRate);
		StartCoroutine (MoveTowardsTarget ());
	}

	IEnumerator GetRandomPosition ()
	{
		Vector3 randPos = RandomNavmeshLocation (_randomPositionRadius);

		if (randPos == null)
		{
			yield return new WaitForSeconds (_getRandomPositionRate);
			StartCoroutine (GetRandomPosition ());
		}
		else
		{
			_target = randPos;
		}
	}

	public void OnDeath ()
	{
		gameObject.SetActive (false);
		// transform.DOScale (Vector3.zero, _spawnDuration).OnComplete (DestroyGO);
	}

	public Vector3 RandomNavmeshLocation (float radius_)
	{
		Vector3 randomDirection = Random.insideUnitSphere * radius_;
		randomDirection += transform.position;
		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition (randomDirection, out hit, radius_, 1))
		{
			finalPosition = hit.position;
		}
		return finalPosition;
	}

}