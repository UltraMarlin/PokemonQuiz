using UnityEditor;

public static class SaveAssetsUtility
{
    [MenuItem("MyUtilities/Save All Assets")]
    public static void SaveAllAssets()
    {
        AssetDatabase.SaveAssets();
        UnityEngine.Debug.Log("Assets saved.");
    }
}
