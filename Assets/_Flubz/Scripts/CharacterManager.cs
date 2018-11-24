using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : MonoBehaviour
{
	public static CharacterManager _instance = null;
	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this) Destroy (gameObject);
	}

	[SerializeField] int _characterCount;
	[SerializeField] Character _characterPrefab;
	[SerializeField] Transform _spawnOrigin;
	[SerializeField] float _spawnRadius;
	List<Character> _characters = new List<Character> ();
	int _charactersLeft;

	private void Start ()
	{
		SpawnCharacters ();
		_charactersLeft = _characterCount;
	}

	private void OnDestroy ()
	{
		_instance = null;
	}

	public void RemoveCharacter (Character char_)
	{
		Destroy (char_.gameObject);
		_charactersLeft--;
		string s = "Villagers left: " + (_charactersLeft - _characterCount / 2).ToString () + " / " + (_characterCount / 2).ToString ();
		PlayerManager._instance.UpdateCharCount (s);
		if (_charactersLeft <= _characterCount / 2)
		{
			PlayerManager._instance.UpdateCharCount ("");
			PlayerManager._instance.GameOver (false, true);
		}
	}

	void SpawnCharacters ()
	{
		for (int i = 0; i < _characterCount; i++)
		{
			Vector3 spawnPos = RandomNavmeshLocation (_spawnRadius, _spawnOrigin.position);
			Character tempC = (Character) Instantiate (_characterPrefab, new Vector3 (spawnPos.x, 0, spawnPos.z), Quaternion.identity);
			_characters.Add (tempC);
		}
	}

	public static Vector3 RandomNavmeshLocation (float radius_, Vector3 initialPos_)
	{
		Vector2 randDir2D = Random.insideUnitCircle * radius_;
		Vector3 randomDirection = new Vector3 (randDir2D.x, 0, randDir2D.y);
		randomDirection += initialPos_;
		NavMeshHit hit;
		Vector3 finalPosition = Vector3.zero;
		if (NavMesh.SamplePosition (randomDirection, out hit, radius_, 1))
		{
			finalPosition = hit.position;
		}
		return finalPosition;
	}
}