using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Rewired;
using Sirenix.OdinInspector;
using SollaraGames.Managers;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] string _buttonToPressToJoinGame = "JoinGame";
	[SerializeField] string _buttonToPauseGame = "PauseGame";
	[SerializeField] Player _playerPrefab;
	[SerializeField] InputMode _pausedInputMode = InputMode.UI;
	InputMode _prePauseInputMode;

	public List<Player> _Players { get; private set; }
	public Action OnPlayerAdded;
	public Action OnPlayerRemoved;
	public int _MaxPlayers { get { return 4; } }

	[SerializeField] TMP_Text _playerCountText;
	[SerializeField] CanvasGroup _canvasGroupPlayerCount;
	[SerializeField] CanvasGroup _canvasGroupPauseMenu;

	List<PlayerMap> _playerMap;
	bool _charsSpawned;

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

	public void OnGameStarted ()
	{
		_canvasGroupPlayerCount.alpha = 0.0f;
		_canvasGroupPlayerCount.DOFade (1.0f, 1.0f).OnComplete (OnPlayerCountFadedIn);
	}

	void OnPlayerCountFadedIn ()
	{
		InputManager._instance.SwitchAllPlayersToInputMode (InputMode.JoinGame);
		_playerMap = new List<PlayerMap> ();
		_Players = new List<Player> ();
		_charsSpawned = false;
	}

	public void OnLevelLoaded ()
	{
		if (!_charsSpawned)
		{
			for (int i = 0; i < _playerMap.Count; i++)
			{
				_charsSpawned = true;
				SpawnPlayer (i);
			}
		}
	}

	void Update ()
	{
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			if (ReInput.players.GetPlayer (i).GetButtonDown (_buttonToPressToJoinGame))
			{
				AssignNextPlayer (i);
			}
			else if (ReInput.players.GetPlayer (i).GetButtonDown (_buttonToPauseGame))
			{
				_canvasGroupPauseMenu.DOFade (1.0f, 1.0f).OnComplete (OnPauseMenu);
			}
		}
	}

	public void LoadMainMenu ()
	{
		UnPauseMenu ();
		_playerCountText.text = 0. ToString () + "/4";
		ApplicationManager._instance.LoadMainMenu ();
	}

	public void OnPauseMenu ()
	{
		InputManager._instance.SwitchAllPlayersToInputMode (_pausedInputMode);
		Time.timeScale = 0;
	}

	public void UnPauseMenu ()
	{
		_canvasGroupPauseMenu.DOFade (0.0f, 1.0f).OnComplete (OnUnPauseMenu);
	}

	void OnUnPauseMenu ()
	{
		Time.timeScale = 1;
		InputManager._instance.SwitchAllPlayersToInputMode (InputMode.Gameplay);
	}

	void AssignNextPlayer (int rewiredPlayerID_)
	{
		if (_playerMap.Count >= _MaxPlayers)
		{
			Debug.LogError ("Max player limit already reached!");
			return;
		}

		_playerMap.Add (new PlayerMap (rewiredPlayerID_, rewiredPlayerID_));
		_playerCountText.text = _playerMap.Count.ToString () + " / 4";
		InputManager._instance.EnablePlayerMap (rewiredPlayerID_, InputMode.Paused);

		if (_playerMap.Count == 4)
		{
			_canvasGroupPlayerCount.DOFade (0.0f, 1.0f).OnComplete (LoadLevel);
		}
	}

	void LoadLevel ()
	{
		ApplicationManager._instance.LoadLevel ();
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
		Player player = (Player) Instantiate (_playerPrefab, Vector2.zero + (UnityEngine.Random.insideUnitCircle * 5.0f), Quaternion.identity);
		player.Initialize (gamePlayerID_);

		if (gamePlayerID_ == 0 || gamePlayerID_ == 2)
		{
			if (gamePlayerID_ == 0) player.SetupUIPosition (false, false);
			else player.SetupUIPosition (true, false);
		}
		else
		{
			if (gamePlayerID_ == 1) player.SetupUIPosition (false, true);
			else if (gamePlayerID_ == 3) player.SetupUIPosition (true, true);
		}

		_Players.Add (player);
	}

	public void RemovePlayer (Player player_)
	{
		Destroy (player_.gameObject);
		_Players = FindObjectsOfType<Player> ().ToList ();
		if (OnPlayerRemoved != null) OnPlayerRemoved.Invoke ();
	}
}