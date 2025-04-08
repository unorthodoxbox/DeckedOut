using UnityEngine;

[ExecuteInEditMode]
//[ExecuteAlways]

public class LightingController : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingProperties Preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    [SerializeField] private Gradient SunColorGradient;

    [SerializeField] private Material skyboxMaterial;

    [SerializeField] private float dayAtmosphereThickness = 0.45f; // daytime value (6-18)
    [SerializeField] private float nightAtmosphereThickness = 0.22f; // nighttime value (0-3, 21-24)

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
            TimeOfDay %= 24; //clamp between 0-24
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
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
    }

    //helper method to change atmosphere thickness - used for dusk/dawn & night adjustments
    private void UpdateAtmosphereThickness(float timePercent)
    {
        if (skyboxMaterial != null)
        {
            if (timePercent >= 0.125 && timePercent <= 0.25) //dawn transitions
            {
                float lerpValue = Mathf.InverseLerp(0.125f, 0.25f, timePercent);
                float thickness = Mathf.Lerp(nightAtmosphereThickness, dayAtmosphereThickness, lerpValue);
                skyboxMaterial.SetFloat("_AtmosphereThickness", thickness);
            }
            else if (timePercent >= 0.75 && timePercent <= 0.875) //dusk transitions
            {
                float lerpValue = Mathf.InverseLerp(0.75f, 0.875f, timePercent);
                float thickness = Mathf.Lerp(dayAtmosphereThickness, nightAtmosphereThickness, lerpValue);
                skyboxMaterial.SetFloat("_AtmosphereThickness", thickness);
            }
            else if (timePercent > 0.25 && timePercent < 0.75) //day
            {
                skyboxMaterial.SetFloat("_AtmosphereThickness", dayAtmosphereThickness);
            }
            else if (timePercent < 0.125 || timePercent > 0.875) //night
            {
                skyboxMaterial.SetFloat("_AtmosphereThickness", nightAtmosphereThickness);
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
