using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSettings : MonoBehaviour
{
    public Light sun;
    public Color skyColor;
    public Color groundColor;
    public float fogStart = 5000f, fogEnd = 9500f;
    public float ambientIntensity = 1f;

    private void Start()
    {
        if (sun != null)
            RenderSettings.sun = sun;
        RenderSettings.skybox.SetColor("_SkyTint", skyColor);
        RenderSettings.skybox.SetColor("_GroundColor", groundColor);
        RenderSettings.fogColor = groundColor;
        RenderSettings.fogStartDistance = fogStart;
        RenderSettings.fogEndDistance = fogEnd;
        RenderSettings.ambientIntensity = ambientIntensity;
    }
}
