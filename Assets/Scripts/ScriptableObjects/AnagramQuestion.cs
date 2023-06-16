using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AnagramQuestion", menuName = "QuestionTypes/Anagram", order = 4)]
public class AnagramQuestion : ScriptableObject, IQuestion
{
    public string pokemonName;
    public string anagramString;
    public Sprite sprite;

    public void OnValidate()
    {
        if (sprite != null)
        {
            pokemonName = sprite.name.Split("_")[1];
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
        }
    }
}
