using UnityEngine;

namespace FPS.VFX
{
    public class ImpactEffectController : MonoBehaviour
    {
        [Header("Particles")]
        [SerializeField] private ParticleSystem impactParticles;
        [SerializeField] private ParticleSystem sparkParticles;

        [Header("Decal")]
        [SerializeField] private GameObject decalPrefab;

        [Header("Settings")]
        [SerializeField] private float lifetime = 2f;

        public void PlayImpact(Vector3 position, Vector3 normal, int materialType = 0)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(normal);

            if (impactParticles != null)
                impactParticles.Play();

            if (sparkParticles != null && materialType == 0)
                sparkParticles.Play();

            if (decalPrefab != null)
            {
                GameObject decal = Instantiate(decalPrefab, position, Quaternion.LookRotation(normal));
                Destroy(decal, lifetime);
            }

            Destroy(gameObject, lifetime);
        }
    }
}