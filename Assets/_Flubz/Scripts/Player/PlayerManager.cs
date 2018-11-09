using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using Sirenix.OdinInspector;
using SollaraGames.Managers;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] string _buttonToPressToJoinGame = "JoinGame";
	[SerializeField] Player _playerPrefab;

	public List<Player> _Players { get; private set; }
	public Action OnPlayerAdded;
	public Action OnPlayerRemoved;
	public int _MaxPlayers { get { return 4; } }

	List<PlayerMap> _playerMap;

	public static PlayerManager _instance = null;
	public Rewired.Player GetRewiredPlayer (int gamePlayerID_)
	{
		if (!Rewired.ReInput.isReady) return null;
		if (_instance == null)
		{
			Debug.LogError ("Not initialized!");
			return null;
		}
		for (int i = 0; i < _instance._playerMap.Count; i++)
		{
			if (_instance._playerMap[i]._gamePlayerID == gamePlayerID_) return ReInput.players.GetPlayer (_instance._playerMap[i]._rewiredPlayerID);
		}
		return null;
	}

	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this)
		{
			Destroy (gameObject);
			return;
		}
	}
	
	public void OnLoadedLevel ()
	{
		_playerMap = new List<PlayerMap> ();
		_Players = new List<Player> ();
	}

	void Update ()
	{
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			if (ReInput.players.GetPlayer (i).GetButtonDown (_buttonToPressToJoinGame))
			{
				AssignNextPlayer (i);
			}
		}
	}

	void AssignNextPlayer (int rewiredPlayerID_)
	{
		if (_playerMap.Count >= _MaxPlayers)
		{
			Debug.LogError ("Max player limit already reached!");
			return;
		}

		_playerMap.Add (new PlayerMap (rewiredPlayerID_, rewiredPlayerID_));

		SpawnPlayer (rewiredPlayerID_);

		InputManager._instance.EnablePlayerMap (rewiredPlayerID_, InputMode.Gameplay);
		if (OnPlayerAdded != null) OnPlayerAdded.Invoke ();
	}

	private class PlayerMap
	{
		public int _rewiredPlayerID;
		public int _gamePlayerID;

		public PlayerMap (int rewiredPlayerId_, int gamePlayerID_)
		{
			this._rewiredPlayerID = rewiredPlayerId_;
			this._gamePlayerID = gamePlayerID_;
		}
	}

	public void SpawnPlayer (int gamePlayerID_)
	{
		Player player = (Player) Instantiate (_playerPrefab, Vector3.zero, Quaternion.identity);
		player._GamePlayerID = gamePlayerID_;
		_Players.Add (player);
	}

	public void RemovePlayer (Player player_)
	{
		Destroy (player_.gameObject);
		_Players = FindObjectsOfType<Player> ().ToList ();
		if (OnPlayerRemoved != null) OnPlayerRemoved.Invoke ();
	}
}