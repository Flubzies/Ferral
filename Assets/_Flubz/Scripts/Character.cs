using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour, ICharacterPlayer
{
	[SerializeField] MinMax _movementUpdateRate;
	[SerializeField] NavMeshAgent _agent;
	[SerializeField] float _spawnWaitTime = 2.0f;
	[SerializeField] float _spawnDuration = 0.5f;
	[SerializeField] float _timeBeforeDestroy = 4f;
	[SerializeField] MinMax _randomPositionRadius;
	[SerializeField] Health _health;
	[SerializeField] RagDoll _ragdoll;

	public int _GamePlayerID { get; set; }

	Vector3 _targetPosition;

	private void Awake ()
	{
		_health.OnDeath += OnDeath;
	}

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
		StartCoroutine (MoveTowardsTarget ());
	}

	IEnumerator MoveTowardsTarget ()
	{
		if (_agent.isOnNavMesh && _targetPosition != Vector3.zero)
		{
			_agent.SetDestination (_targetPosition);
		}
		else
		{
			yield return new WaitForSeconds (_movementUpdateRate.GetRandom);
			StartCoroutine (MoveTowardsTarget ());
		}

		yield return new WaitForSeconds (_movementUpdateRate.GetRandom);
		_targetPosition = CharacterManager.RandomNavmeshLocation (_randomPositionRadius.GetRandom, transform.position);
		StartCoroutine (MoveTowardsTarget ());
	}

	public void HasBeenKilled (float timeBeforeKill_)
	{
		StartCoroutine (KillWait (timeBeforeKill_));
	}

	IEnumerator KillWait (float timeBeforeKill_)
	{
		yield return new WaitForSeconds (timeBeforeKill_);
		_ragdoll.ActivateRagDoll ();
		yield return new WaitForSeconds (_timeBeforeDestroy);
		_health.Damage (1000);
	}

	public void OnDeath ()
	{
		Debug.Log ("An innocent villager was killed!");
		_health.OnDeath -= OnDeath;
		_ragdoll._ragdollGFX.transform.DOScale (Vector3.zero, _spawnDuration).OnComplete (DestroyGO);
	}

	void DestroyGO ()
	{
		Destroy (gameObject);
	}

}

[System.Serializable]
public class RagDoll
{
	public Rigidbody _rb;
	public Collider _characterCollider;
	public GameObject _characterGFX;
	public GameObject _ragdollGFX;
	public NavMeshAgent _agent;

	public void ActivateRagDoll ()
	{
		_rb.isKinematic = true;
		_characterCollider.enabled = false;
		if (_agent != null) _agent.enabled = false;
		_characterGFX.SetActive (false);
		_ragdollGFX.SetActive (true);
	}
}