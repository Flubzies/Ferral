using UnityEngine;

namespace Managers
{
	[RequireComponent (typeof (CanvasFader))]
	public class Menu : MonoBehaviour
	{
		[SerializeField] protected CanvasFader _canvasFader;
		[SerializeField] bool _disableCanvasGroupOnStart;
		bool _menuIsOpen;

		public void ToggleMenu ()
		{
			if (_menuIsOpen) CloseMenu ();
			else OpenMenu ();
		}

		public void OpenMenu ()
		{
			if (_menuIsOpen) return;
			_canvasFader.FadeCanvasIn ();
			_canvasFader.OnFadeInComplete += OnMenuOpened;
			_menuIsOpen = true;
		}

		public void CloseMenu ()
		{
			if (!_menuIsOpen) return;
			_canvasFader.FadeCanvasOut ();
			_canvasFader.OnFadeOutComplete += OnMenuClosed;
		}

		void OnMenuOpened ()
		{
			_canvasFader.OnFadeInComplete -= OnMenuOpened;
		}

		void OnMenuClosed ()
		{
			_canvasFader.OnFadeOutComplete -= OnMenuClosed;
			_menuIsOpen = false;
		}

		private void OnDestroy ()
		{
			_canvasFader.OnFadeInComplete -= OnMenuOpened;
			_canvasFader.OnFadeOutComplete -= OnMenuClosed;
		}
	}
}