using UnityEngine;
using UnityEditor;

public class ParticleSetCreateAsset
{
    [MenuItem("Assets/Create/ParticleSet")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<ParticleSet>();
    }
}
