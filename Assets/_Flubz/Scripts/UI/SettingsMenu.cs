using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class SettingsMenu : Menu
	{

		public static SettingsMenu _Instance = null;

		void Awake ()
		{
			if (_Instance == null) _Instance = this;
			else if (_Instance != this) Destroy (_Instance);
		}

		public void OnClickRestart ()
		{
			// SceneTransition._Instance.LoadScene (SceneManager.GetActiveScene ().name);
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			CloseMenu ();
		}

		private void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				ToggleMenu ();
			}
		}

		public void OnClickResume ()
		{
			CloseMenu ();
		}

		public void OnClickExit ()
		{
			Debug.Log ("Exiting.");
			Application.Quit ();
		}
	}
}