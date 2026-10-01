using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class CrosshairController : MonoBehaviour
    {
        [Header("Crosshair Elements")]
        [SerializeField] private RectTransform centerDot;
        [SerializeField] private RectTransform[] crosshairLines = new RectTransform[4];

        [Header("Style")]
        [SerializeField] private Color idleColor = new Color(0.3f, 0.9f, 1f, 0.6f);
        [SerializeField] private Color fireColor = new Color(1f, 0.4f, 0.2f, 1f);
        [SerializeField] private float baseSeparation = 20f;
        [SerializeField] private float maxSeparation = 50f;

        [Header("Animation")]
        [SerializeField] private float spreadSpeed = 15f;
        [SerializeField] private float spreadReturnSpeed = 8f;
        [SerializeField] private float fireFlashDuration = 0.1f;

        private float currentSeparation;
        private float fireFlashTimer;
        private Image[] lineImages;

        private void Start()
        {
            lineImages = new Image[crosshairLines.Length];
            for (int i = 0; i < crosshairLines.Length; i++)
            {
                lineImages[i] = crosshairLines[i].GetComponent<Image>();
                if (lineImages[i] != null)
                    lineImages[i].color = idleColor;
            }

            currentSeparation = baseSeparation;
        }

        public void OnFire()
        {
            currentSeparation = Mathf.Min(currentSeparation + 10f, maxSeparation);
            fireFlashTimer = fireFlashDuration;
        }

        private void Update()
        {
            currentSeparation = Mathf.Lerp(currentSeparation, baseSeparation, Time.deltaTime * spreadReturnSpeed);

            if (centerDot != null)
                centerDot.anchoredPosition = Vector2.zero;

            if (crosshairLines.Length >= 4)
            {
                crosshairLines[0].anchoredPosition = new Vector2(0, currentSeparation);
                crosshairLines[1].anchoredPosition = new Vector2(0, -currentSeparation);
                crosshairLines[2].anchoredPosition = new Vector2(currentSeparation, 0);
                crosshairLines[3].anchoredPosition = new Vector2(-currentSeparation, 0);
            }

            if (fireFlashTimer > 0)
            {
                fireFlashTimer -= Time.deltaTime;
                for (int i = 0; i < lineImages.Length; i++)
                {
                    if (lineImages[i] != null)
                        lineImages[i].color = Color.Lerp(fireColor, idleColor, fireFlashTimer / fireFlashDuration);
                }
            }
        }
    }
}