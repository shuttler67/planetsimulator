using UnityEditor;

public class FloatReferenceAsset
{
    [MenuItem("Assets/Create/ScriptableObject/FloatReference")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<FloatReference>();
    }
}