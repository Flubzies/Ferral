using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class SceneTransition : Menu
    {
        [SerializeField] GameObject _loadingPanel;
        [SerializeField] Image _loadingAmount;
        string _sceneToLoad = null;

        public static SceneTransition _Instance = null;

        void Awake ()
        {
            if (_Instance == null) _Instance = this;
            else if (_Instance != this) Destroy (_Instance);
        }

        private void Start ()
        {
            _loadingPanel.SetActive (false);
        }

        public void FadeFromScene ()
        {
            _canvasFader.FadeCanvasOut ();
        }

        public void LoadScene (string scene_ = null)
        {
            _canvasFader.FadeCanvasIn ();
            if (_sceneToLoad != null)
            {
                _sceneToLoad = scene_;
                _canvasFader.OnFadeInComplete += OnFadedIn;
            }
        }

        void OnFadedIn ()
        {
            _canvasFader.OnFadeInComplete -= OnFadedIn;
            StartCoroutine (LoadAsync (_sceneToLoad));
        }

        IEnumerator LoadAsync (string _sceneToLoad)
        {
            // yield return new WaitForSecondsRealtime (_fadeDuration);
            _loadingPanel.SetActive (true);

            AsyncOperation operation = SceneManager.LoadSceneAsync (_sceneToLoad);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01 (operation.progress / 0.9f);
                _loadingAmount.fillAmount = progress;
                yield return null;
            }

            CloseMenu ();
            _loadingPanel.SetActive (false);
        }

        private void OnDestroy ()
        {
            _canvasFader.OnFadeInComplete -= OnFadedIn;
        }
    }
}