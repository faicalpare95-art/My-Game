using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class FPSHUDController : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private CanvasGroup hudRoot;
        [SerializeField] private Image crosshairCenter;
        [SerializeField] private Image crosshairLeft;
        [SerializeField] private Image crosshairRight;
        [SerializeField] private Image crosshairTop;
        [SerializeField] private Image crosshairBottom;
        [SerializeField] private Text ammoText;
        [SerializeField] private Text healthText;

        [Header("Colors")]
        [SerializeField] private Color crosshairColor = new Color(0.5f, 0.9f, 1f, 1f);
        [SerializeField] private Color accentColor = new Color(0.2f, 0.8f, 0.9f, 1f);

        private void Reset()
        {
            if (hudRoot == null)
            {
                hudRoot = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            ApplyColors();
        }

        private void ApplyColors()
        {
            if (crosshairCenter != null) crosshairCenter.color = crosshairColor;
            if (crosshairLeft != null) crosshairLeft.color = crosshairColor;
            if (crosshairRight != null) crosshairRight.color = crosshairColor;
            if (crosshairTop != null) crosshairTop.color = crosshairColor;
            if (crosshairBottom != null) crosshairBottom.color = crosshairColor;
            if (ammoText != null) ammoText.color = accentColor;
            if (healthText != null) healthText.color = accentColor;
        }

        public void SetAmmo(int currentAmmo, int reserveAmmo)
        {
            if (ammoText != null)
            {
                ammoText.text = currentAmmo + " / " + reserveAmmo;
            }
        }

        public void SetHealth(float health)
        {
            if (healthText != null)
            {
                healthText.text = "HP " + Mathf.RoundToInt(health);
            }
        }
    }
}
