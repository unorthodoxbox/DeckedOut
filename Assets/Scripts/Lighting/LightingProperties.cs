using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Properties", menuName = "Sciptables/Lighting Properties", order = 1)]

public class LightingProperties : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;

    //code
}
