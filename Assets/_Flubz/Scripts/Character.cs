using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
	[SerializeField] MinMax _movementUpdateRate;
	[SerializeField] NavMeshAgent _agent;
	[SerializeField] float _spawnWaitTime = 2.0f;
	[SerializeField] float _spawnDuration = 0.5f;
	[SerializeField] MinMax _randomPositionRadius;

	public int _GamePlayerID { get; set; }

	Vector3 _targetPosition;

	private void Start ()
	{
		_agent.enabled = false;
		StartCoroutine (SpawnWaitTime ());
	}

	IEnumerator SpawnWaitTime ()
	{
		transform.localScale = Vector3.zero;
		transform.DOScale (Vector3.one, _spawnDuration);
		yield return new WaitForSeconds (_spawnWaitTime);
		_agent.enabled = true;
		if (_agent.isOnNavMesh)
		{
			_agent.Warp (transform.position);
		}
		else
		{
			OnDeath ();
		}
		StartCoroutine (MoveTowardsTarget ());
	}

	IEnumerator MoveTowardsTarget ()
	{
		if (_agent.isOnNavMesh)
		{
			_agent.SetDestination (_targetPosition);
		}
		else
		{
			OnDeath ();
		}

		yield return new WaitForSeconds (_movementUpdateRate.GetRandom);
		_targetPosition = RandomNavmeshLocation (_randomPositionRadius.GetRandom);
		StartCoroutine (MoveTowardsTarget ());
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