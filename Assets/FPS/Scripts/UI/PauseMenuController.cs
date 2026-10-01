using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FPS.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        [Header("Animation")]
        [SerializeField] private float fadeDuration = 0.2f;

        private bool isPaused;

        private void Start()
        {
            if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
            if (restartButton != null) restartButton.onClick.AddListener(Restart);
            if (quitButton != null) quitButton.onClick.AddListener(QuitToMenu);

            if (pausePanel != null) pausePanel.alpha = 0f;
            isPaused = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        private void Pause()
        {
            isPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
                StartCoroutine(FadePanel(pausePanel, 1f));
        }

        private void Resume()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                StartCoroutine(FadePanel(pausePanel, 0f));
        }

        private void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("IntroMenu");
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
    }
}