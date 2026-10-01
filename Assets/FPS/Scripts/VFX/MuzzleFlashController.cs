using UnityEngine;

namespace FPS.VFX
{
    public class MuzzleFlashController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParticleSystem muzzleParticles;
        [SerializeField] private Light muzzleLight;
        [SerializeField] private MeshRenderer muzzleFlashRenderer;

        [Header("Settings")]
        [SerializeField] private float flashDuration = 0.05f;
        [SerializeField] private float lightIntensity = 2f;
        [SerializeField] private float lightRange = 15f;

        private float flashTimer;

        public void Fire()
        {
            if (muzzleParticles != null)
                muzzleParticles.Play();

            if (muzzleLight != null)
            {
                muzzleLight.intensity = lightIntensity;
                muzzleLight.range = lightRange;
            }

            if (muzzleFlashRenderer != null)
                muzzleFlashRenderer.enabled = true;

            flashTimer = flashDuration;
        }

        private void Update()
        {
            if (flashTimer > 0)
            {
                flashTimer -= Time.deltaTime;
            }
            else
            {
                if (muzzleLight != null)
                    muzzleLight.intensity = 0f;
                if (muzzleFlashRenderer != null)
                    muzzleFlashRenderer.enabled = false;
            }
        }
    }
}