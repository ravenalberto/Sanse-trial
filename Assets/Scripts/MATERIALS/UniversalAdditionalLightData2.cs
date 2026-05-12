using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public static class LightExtensions
    {
        public static UniversalAdditionalLightData2 GetUniversalAdditionalLightData(this Light light)
        {
            var gameObject = light.gameObject;
            if (!gameObject.TryGetComponent<UniversalAdditionalLightData2>(out var lightData))
                lightData = gameObject.AddComponent<UniversalAdditionalLightData2>();

            return lightData;
        }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public partial class UniversalAdditionalLightData2 : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Header("Resident Evil Style Settings")]
        [SerializeField] bool m_UsePipelineSettings = true;
        public bool usePipelineSettings { get => m_UsePipelineSettings; set => m_UsePipelineSettings = value; }

        [Header("Brightness Tweak")]
        [Tooltip("Increase this value to make the light lighter and less dark.")]
        [Range(1f, 5f)]
        public float intensityMultiplier = 1f;

        // Shadow Resolution Tiers
        public static readonly int ShadowResolutionLow = 256;
        public static readonly int ShadowResolutionMedium = 512;
        public static readonly int ShadowResolutionHigh = 1024;

        [SerializeField] int m_AdditionalLightsShadowResolutionTier = 2;
        public int additionalLightsShadowResolutionTier => m_AdditionalLightsShadowResolutionTier;

        [SerializeField] bool m_CustomShadowLayers = false;
        public bool customShadowLayers
        {
            get => m_CustomShadowLayers;
            set { if (m_CustomShadowLayers != value) { m_CustomShadowLayers = value; SyncLayers(); } }
        }

        [SerializeField] Vector2 m_LightCookieSize = Vector2.one;
        public Vector2 lightCookieSize { get => m_LightCookieSize; set => m_LightCookieSize = value; }

        [SerializeField] SoftShadowQuality m_SoftShadowQuality = SoftShadowQuality.UsePipelineSettings;
        public SoftShadowQuality softShadowQuality { get => m_SoftShadowQuality; set => m_SoftShadowQuality = value; }

        [Header("Rendering Layers")]
        [SerializeField] RenderingLayerMask m_RenderingLayers = RenderingLayerMask.defaultRenderingLayerMask;
        public RenderingLayerMask renderingLayers { get => m_RenderingLayers; set { m_RenderingLayers = value; SyncLayers(); } }

        private Light m_Light;
        internal Light cachedLight
        {
            get
            {
                if (!m_Light) TryGetComponent(out m_Light);
                return m_Light;
            }
        }

        void SyncLayers()
        {
            if (cachedLight != null)
            {
                cachedLight.renderingLayerMask = m_RenderingLayers;
                // If you want the multiplier to affect the actual light component:
                // cachedLight.intensity *= intensityMultiplier; 
            }
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { SyncLayers(); }
    }
}