using UnityEngine;
using UnityEditor;

public class SoundSetsCreateAsset
{
    [MenuItem("Assets/Create/SoundSets")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<SoundSets>();
    }
}
