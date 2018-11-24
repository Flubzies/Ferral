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
	[SerializeField] Transform _spawnPos;
	[SerializeField] float _spawnRadius;
	InputMode _prePauseInputMode;

	public List<Player> _Players { get; private set; }
	public Player GetWolf
	{
		get
		{
			if (_Players != null)
			{
				foreach (var item in _Players)
				{
					if (item._IsWereWolf)
						return item;
				}
			}
			return null;
		}
	}

	public Action OnPlayerAdded;
	public Action OnPlayerRemoved;
	public int _MaxPlayers { get { return 4; } }

	[SerializeField] TMP_Text _playerCountText;
	[SerializeField] CanvasGroup _canvasGroupPlayerCount;
	[SerializeField] CanvasGroup _canvasGroupPauseMenu;
	[SerializeField] bool _straightToLevel;
	[TextArea][SerializeField] string _gameWonWolfText;
	[TextArea][SerializeField] string _gameWonWolfTextLostManyPeople;
	[TextArea][SerializeField] string _gameWonHumanText;
	[SerializeField] TMP_Text _victoryText;
	[SerializeField] TMP_Text _informationText;
	[SerializeField] TMP_Text _charCount;

	public void UpdateCharCount (string s)
	{
		_charCount.text = s;
	}

	List<PlayerMap> _playerMap;
	bool _charsSpawned;
	int _toBecomeWereWolf;

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
		if (_straightToLevel) _canvasGroupPlayerCount.alpha = 0.0f;
		else LoadMainMenu ();
	}

	public void OnGameStarted ()
	{
		if (_straightToLevel) _straightToLevel = false;
		PlayerManager._instance.UpdateCharCount ("");
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
		_toBecomeWereWolf = UnityEngine.Random.Range (0, 4);
		if (!_charsSpawned && _playerMap != null)
		{
			for (int i = 0; i < _playerMap.Count; i++)
			{
				_charsSpawned = true;
				SpawnPlayer (i);
			}
		}
		else if (_straightToLevel)
		{
			OnPlayerCountFadedIn ();
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
				StartPause ();
			}
		}
	}

	bool _isDisplaying;
	[SerializeField] float _textFadeTime = 0.4f;

	public void InformationText (string s)
	{
		if (_isDisplaying) return;
		_isDisplaying = true;
		_informationText.text = s;
		_informationText.DOFade (1.0f, _textFadeTime).OnComplete (FadeTextOut);
	}

	void FadeTextOut ()
	{
		_informationText.DOFade (0.0f, _textFadeTime);
		_isDisplaying = false;
	}

	public void LoadMainMenu ()
	{
		UnPauseMenu ();
		_playerCountText.text = 0. ToString () + " / 4";
		ApplicationManager._instance.LoadMainMenu ();
	}

	void StartPause (string s = "Game Paused")
	{
		_victoryText.text = s;
		_canvasGroupPauseMenu.DOFade (1.0f, 1.0f).OnComplete (OnPauseMenu);
	}

	public void OnPauseMenu ()
	{
		InputManager._instance.SwitchAllPlayersToInputMode (_pausedInputMode);
		// Time.timeScale = 0;
	}

	public void UnPauseMenu ()
	{
		_canvasGroupPauseMenu.DOFade (0.0f, 1.0f).OnComplete (OnUnPauseMenu);
	}

	void OnUnPauseMenu ()
	{
		// Time.timeScale = 1;
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

		if (_straightToLevel) SpawnPlayer (rewiredPlayerID_);

		if (_playerMap.Count == 4 && !_straightToLevel)
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
		Vector2 spawnPos = UnityEngine.Random.insideUnitCircle * _spawnRadius;
		Player player = (Player) Instantiate (_playerPrefab, _spawnPos.position + new Vector3 (spawnPos.x, 0, spawnPos.y), Quaternion.identity);
		if (gamePlayerID_ == _toBecomeWereWolf) player.Initialize (gamePlayerID_, true);
		else player.Initialize (gamePlayerID_);

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

		if (_straightToLevel) InputManager._instance.EnablePlayerMap (gamePlayerID_, InputMode.Gameplay);

		_Players.Add (player);
	}

	public void RemovePlayer (Player player_)
	{
		InputManager._instance.EnablePlayerMap (player_._GamePlayerID, InputMode.Paused);
		_Players = FindObjectsOfType<Player> ().ToList ();
		CheckGameWon ();
		if (OnPlayerRemoved != null) OnPlayerRemoved.Invoke ();
	}

	void CheckGameWon ()
	{
		if (_Players.Count == 1 && GetWolf)
		{
			GameOver (false);
		}
	}

	public void GameOver (bool wonByHumans_, bool lossByLostManyPeople_ = false)
	{
		if (lossByLostManyPeople_)
		{
			StartPause (_gameWonWolfTextLostManyPeople);
			return;
		}
		if (wonByHumans_) StartPause (_gameWonHumanText);
		else StartPause (_gameWonWolfText);
	}
}