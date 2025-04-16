using UnityEngine;

[ExecuteInEditMode]
//[ExecuteAlways]

public class LightingController : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingProperties Preset;
    [SerializeField, Range(0, 120)] private float TimeOfDay;
    [SerializeField] private Gradient SunColorGradient;

    [SerializeField] private Material skyboxMaterial;

    [SerializeField] private float dayAtmosphereThickness = 0.45f; // daytime value (7.5-16.5)
    [SerializeField] private float nightAtmosphereThickness = 0.22f; // nighttime value (0-4.5, 19.5-24)
    [SerializeField] private Color daySkyTint = new Color32(0x06, 0x6F, 0x6C, 0xFF);
    [SerializeField] private Color nightSkyTint = new Color32(0x33, 0x00, 0xC0, 0xFF);
    [SerializeField] private Color dayGroundColor = new Color32(0x75, 0x64, 0x98, 0xFF);
    [SerializeField] private Color nightGroundColor = new Color32(0x16, 0x08, 0x33, 0xFF);

    void Start()
    {
        Debug.Log("Current skybox material: " + RenderSettings.skybox);
        Debug.Log("Skybox shader: " + RenderSettings.skybox.shader.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 120; //clamp between 0-24
            UpdateLighting(TimeOfDay / 120f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 120f);
        }
    }

    //main lighting change method
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //minimum night time lighting
        if (timePercent < 0.2f)
        {
            RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);
        }

        if (DirectionalLight != null)
        {
            DirectionalLight.color = SunColorGradient.Evaluate(timePercent);
            //DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

        UpdateAtmosphereThickness(timePercent);
        UpdateSkyboxColors(timePercent);
    }

    //helper method to change atmosphere thickness - used for dusk/dawn & night adjustments
    private void UpdateAtmosphereThickness(float timePercent)
    {
        if (skyboxMaterial != null)
        {
            if (timePercent >= 0.1875 && timePercent <= 0.3125) //dawn transitions
            {
                float lerpValue = Mathf.InverseLerp(0.1875f, 0.3125f, timePercent);
                float thickness = Mathf.Lerp(nightAtmosphereThickness, dayAtmosphereThickness, lerpValue);
                skyboxMaterial.SetFloat("_AtmosphereThickness", thickness);
            }
            else if (timePercent >= 0.6875 && timePercent <= 0.8125) //dusk transitions
            {
                float lerpValue = Mathf.InverseLerp(0.6875f, 0.8125f, timePercent);
                float thickness = Mathf.Lerp(dayAtmosphereThickness, nightAtmosphereThickness, lerpValue);
                skyboxMaterial.SetFloat("_AtmosphereThickness", thickness);
            }
            else if (timePercent > 0.3125 && timePercent < 0.6875) //day
            {
                skyboxMaterial.SetFloat("_AtmosphereThickness", dayAtmosphereThickness);
            }
            else if (timePercent < 0.1875 || timePercent > 0.8125) //night
            {
                skyboxMaterial.SetFloat("_AtmosphereThickness", nightAtmosphereThickness);
            }
        }
    }

    //updates the skybox ting & ground color based on time of day
    private void UpdateSkyboxColors(float timePercent)
    {
        if (skyboxMaterial != null)
        {
            if (timePercent >= 0.1875 && timePercent <= 0.3125) //dawn transitions
            {
                float lerpValue = Mathf.InverseLerp(0.1875f, 0.3125f, timePercent);
                RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(nightSkyTint, daySkyTint, lerpValue));
                RenderSettings.skybox.SetColor("_GroundColor", Color.Lerp(nightGroundColor, dayGroundColor, lerpValue));
                DynamicGI.UpdateEnvironment();
            }
            else if (timePercent >= 0.6875 && timePercent <= 0.8125) //dusk transitions
            {
                float lerpValue = Mathf.InverseLerp(0.6875f, 0.8125f, timePercent);
                RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(daySkyTint, nightSkyTint, lerpValue));
                RenderSettings.skybox.SetColor("_GroundColor", Color.Lerp(dayGroundColor, nightGroundColor, lerpValue));
                DynamicGI.UpdateEnvironment();
            }
            else if (timePercent > 0.3125 && timePercent < 0.6875) //day
            {
                RenderSettings.skybox.SetColor("_SkyTint", daySkyTint);
                RenderSettings.skybox.SetColor("_GroundColor", dayGroundColor);
                DynamicGI.UpdateEnvironment();
            }
            else if (timePercent < 0.1875 || timePercent > 0.8125) //night
            {
                RenderSettings.skybox.SetColor("_SkyTint", nightSkyTint);
                RenderSettings.skybox.SetColor("_GroundColor", nightGroundColor);
                DynamicGI.UpdateEnvironment();
            }
        }
    }

    //if no directional light is set, find one
    private void OnValidate()
    {
        if (DirectionalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }   
}
