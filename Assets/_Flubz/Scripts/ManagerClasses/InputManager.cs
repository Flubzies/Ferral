using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapDefault = "Default";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapMainMenu = "MCMainMenu";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapJoinGame = "MCJoinGame";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapGameplay = "MCGameplay";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapUI = "MCUI";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapInGameUI = "MCInGameUI";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapPaused = "MCPaused";
	[SerializeField][FoldoutGroup ("Maps Categories")][ReadOnly] string _mapLoading = "MCLoading";

	public string _Default { get { return _mapDefault; } }
	public string _MainMenu { get { return _mapMainMenu; } }
	public string _JoinGame { get { return _mapJoinGame; } }
	public string _Gameplay { get { return _mapGameplay; } }
	public string _UI { get { return _mapUI; } }
	public string _InGameUI { get { return _mapInGameUI; } }
	public string _Paused { get { return _mapPaused; } }
	public string _Loading { get { return _mapLoading; } }

	public static InputManager _instance = null;
	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this) Destroy (gameObject);
	}

	public void EnablePlayerMap (int playerID_, InputMode inputMode_, bool disableAllOtherMaps_ = true)
	{
		if (disableAllOtherMaps_) ReInput.players.GetPlayer (playerID_).controllers.maps.SetAllMapsEnabled (false);
		ReInput.players.GetPlayer (playerID_).controllers.maps.SetMapsEnabled (true, InputModeString (inputMode_));
	}

	public void SwitchAllPlayersToInputMode (InputMode inputMode_)
	{
		for (int i = 0; i < PlayerManager._instance._MaxPlayers; i++)
		{
			ReInput.players.GetPlayer (i).controllers.maps.SetAllMapsEnabled (false);
			ReInput.players.GetPlayer (i).controllers.maps.SetMapsEnabled (true, InputModeString (inputMode_));
		}
	}

	string InputModeString (InputMode inputMode_)
	{
		switch (inputMode_)
		{
			case InputMode.Undefined:
				return _Default;
			case InputMode.MainMenu:
				return _MainMenu;
			case InputMode.JoinGame:
				return _JoinGame;
			case InputMode.Gameplay:
				return _Gameplay;
			case InputMode.UI:
				return _UI;
			case InputMode.InGameUI:
				return _InGameUI;
			case InputMode.Paused:
				return _Paused;
			case InputMode.Loading:
				return _Loading;
			default:
				return _Default;
		}
	}
}

public enum InputMode
{
	Undefined,
	MainMenu,
	JoinGame,
	Gameplay,
	UI,
	InGameUI,
	Paused,
	Loading
}