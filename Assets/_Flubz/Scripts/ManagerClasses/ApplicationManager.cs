using UnityEngine;
using UnityEngine.SceneManagement;

namespace SollaraGames.Managers
{
	public class ApplicationManager : MonoBehaviour
	{
		public static ApplicationManager _instance = null;
		[SerializeField] SceneInputMode _scene;
		[SerializeField] InputMode _defaultInputMode;

		void Awake ()
		{
			if (_instance == null) _instance = this;
			else if (_instance != this) Destroy (gameObject);
			DontDestroyOnLoad (gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void RestartLevel ()
		{
			// 
		}

		public void LoadSomething ()
		{
			SceneManager.LoadScene (_scene._sceneName);
		}

		void OnSceneLoaded (Scene scene, LoadSceneMode mode)
		{

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