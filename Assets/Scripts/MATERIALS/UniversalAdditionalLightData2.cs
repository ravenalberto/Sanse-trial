using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Adjusts lighting to be like a normal building that is almost pitch black.
    /// Deep charcoal tones for an "unlit" feeling.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class AtmosphericLightController : MonoBehaviour
    {
        [Header("Atmosphere Settings")]
        [Tooltip("Deep charcoal interior color for unlit areas.")]
        public Color buildingAmbientColor = new Color(0.15f, 0.15f, 0.18f); // Drastically darkened from 0.5

        [Range(0f, 2f)]
        [Tooltip("Overall brightness of the building. Lower values create an unlit feeling.")]
        public float brightnessLevel = 0.35f; // Reduced from 0.6 to 0.35

        [Header("Local Light Settings")]
        [Tooltip("Intensity of the lights. Kept low to avoid looking like a bright torch.")]
        public float softIntensity = 0.6f; // Reduced from 1.0 to 0.6

        private Light _light;
        private Light cachedLight => _light ??= GetComponent<Light>();

        private void OnValidate() => ApplyBuildingSettings();
        private void Awake() => ApplyBuildingSettings();

        public void ApplyBuildingSettings()
        {
            if (cachedLight == null) return;

            // 1. Set the light color to a very pale, dim grey-white
            cachedLight.color = new Color(0.8f, 0.8f, 0.85f);

            // 2. Disable shadows for performance/brightness control
            cachedLight.shadows = LightShadows.None;

            // 3. Set the local intensity (very dim)
            cachedLight.intensity = softIntensity;

            // 4. Global Ambient Adjustment
            // Flat mode with very low values creates the "no lights" atmosphere
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = buildingAmbientColor * brightnessLevel;

            // 5. Keep range broad but the intensity dim
            if (cachedLight.type == LightType.Point)
            {
                cachedLight.range = 15f; // Reduced range to keep light localized
            }
        }
    }
}