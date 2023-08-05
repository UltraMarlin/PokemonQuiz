using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "AnagramQuestion", menuName = "QuestionTypes/Anagram", order = 4)]
public class AnagramQuestion : IQuestion
{
    public string anagramString;
    public Sprite sprite;
    public PokemonGen pokemonGen;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        if (sprite != null)
        {
            pokemonName = sprite.name;
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
        }
    }
#endif
}
