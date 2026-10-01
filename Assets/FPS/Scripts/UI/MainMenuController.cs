using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FPS.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Panels")]
        [SerializeField] private CanvasGroup menuPanel;
        [SerializeField] private CanvasGroup settingsPanel;

        [Header("Settings")]
        [SerializeField] private Slider qualitySlider;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Animation")]
        [SerializeField] private float fadeDuration = 0.3f;

        private void Start()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

            if (menuPanel != null) menuPanel.alpha = 1f;
            if (settingsPanel != null) settingsPanel.alpha = 0f;
        }

        private void OnPlayClicked()
        {
            StartCoroutine(FadeAndLoad("MainScene"));
        }

        private void OnSettingsClicked()
        {
            if (menuPanel != null)
                StartCoroutine(FadePanel(menuPanel, 0f));
            if (settingsPanel != null)
                StartCoroutine(FadePanel(settingsPanel, 1f));
        }

        private void OnQuitClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private System.Collections.IEnumerator FadePanel(CanvasGroup panel, float targetAlpha)
        {
            float elapsed = 0f;
            float startAlpha = panel.alpha;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                panel.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }
            panel.alpha = targetAlpha;
        }

        private System.Collections.IEnumerator FadeAndLoad(string sceneName)
        {
            if (menuPanel != null)
                yield return StartCoroutine(FadePanel(menuPanel, 0f));
            SceneManager.LoadScene(sceneName);
        }
    }
}