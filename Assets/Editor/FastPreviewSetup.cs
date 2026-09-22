using UnityEditor;
using UnityEngine;

public static class FastPreviewSetup
{
    [MenuItem("Oops Dont Drop It/Apply Fast Preview Settings")]
    public static void Apply()
    {
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.shadowDistance = 0f;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.softParticles = false;
        QualitySettings.particleRaycastBudget = 16;

        PlayerSettings.MTRendering = true;

        Debug.Log("Fast Preview enabled: shadows/AA/reflection extras disabled for smoother Unity Editor testing.");
    }

    [MenuItem("Oops Dont Drop It/Restore Balanced Preview Settings")]
    public static void Restore()
    {
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 2;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowDistance = 30f;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        QualitySettings.realtimeReflectionProbes = true;
        QualitySettings.softParticles = true;

        Debug.Log("Balanced Preview restored.");
    }
}
