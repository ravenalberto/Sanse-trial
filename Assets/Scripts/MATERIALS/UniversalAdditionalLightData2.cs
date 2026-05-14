using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Rendering.Universal
{
    public static class LightExtensions
    {
        public static UniversalAdditionalLightData2 GetUniversalAdditionalLightData(this Light light)
        {
            if (!light.TryGetComponent<UniversalAdditionalLightData2>(out var lightData))
                lightData = light.gameObject.AddComponent<UniversalAdditionalLightData2>();

            return lightData;
        }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light))]
    public class UniversalAdditionalLightData2 : MonoBehaviour
    {
        [Tooltip("When true, it boosts the brightness based on the multiplier below.")]
        public bool boostBrightness = true;

        [Range(1f, 5f)]
        public float intensityMultiplier = 1.5f;

        private Light _light;
        private Light cachedLight => _light ??= GetComponent<Light>();

        private float _baseIntensity = -1f;

        private void OnValidate() => ApplySettings();
        private void Awake() => ApplySettings();

        public void ApplySettings()
        {
            if (cachedLight == null) return;

            // 1. Force Shadows OFF for maximum performance ("making it light")
            cachedLight.shadows = LightShadows.None;

            // 2. Handle Intensity Logic
            if (boostBrightness)
            {
                // Capture the original intensity once so we don't multiply infinitely
                if (_baseIntensity < 0) _baseIntensity = cachedLight.intensity;

                cachedLight.intensity = _baseIntensity * intensityMultiplier;
            }
            else if (_baseIntensity >= 0)
            {
                // Reset to normal if boost is toggled off
                cachedLight.intensity = _baseIntensity;
            }
        }
    }
}