using UnityEngine;
using UnityEngine.SceneManagement;

namespace SollaraGames.Managers
{
	public class ApplicationManager : MonoBehaviour
	{
		public static ApplicationManager _instance = null;
		[SerializeField] SceneInputMode _mainMenu;
		[SerializeField] SceneInputMode _level;
		[SerializeField] InputMode _defaultInputMode;

		void Awake ()
		{
			if (_instance == null) _instance = this;
			else if (_instance != this) Destroy (gameObject);
			DontDestroyOnLoad (gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void LoadLevel ()
		{
			SceneManager.LoadScene (_level._sceneName);
		}

		public void LoadMainMenu ()
		{
			SceneManager.LoadScene (_mainMenu._sceneName);
		}

		void OnSceneLoaded (Scene scene, LoadSceneMode mode)
		{
			Debug.Log (scene.name);
			if (scene.name == _mainMenu._sceneName)
			{
				InputManager._instance.SwitchAllPlayersToInputMode (_mainMenu._inputMode);
				PlayerManager._instance.OnGameStarted ();
			}
			else if (scene.name == _level._sceneName)
			{
				InputManager._instance.SwitchAllPlayersToInputMode (_level._inputMode);
				PlayerManager._instance.OnLevelLoaded ();
			}
		}

		private void OnDestroy ()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		[System.Serializable]
		struct SceneInputMode
		{
			public string _sceneName;
			public InputMode _inputMode;
		};

	}
}