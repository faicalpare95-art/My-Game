using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FPS.Graphics
{
    public class GraphicsSettingsBootstrap : MonoBehaviour
    {
        [Header("URP Settings")]
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        private void Awake()
        {
            if (urpAsset == null)
            {
                urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
            }

            if (urpAsset != null)
            {
                urpAsset.msaaSampleCount = 4;
                urpAsset.supportsHDR = true;
                urpAsset.shadowDistance = 100f;
                urpAsset.mainLightShadowmapResolution = UniversalRenderPipelineAsset.ShadowResolution._2048;
                urpAsset.additionalLightsShadowmapResolution = UniversalRenderPipelineAsset.ShadowResolution._1024;
                urpAsset.renderScale = 1.1f;
            }

            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 144;
        }
    }
}