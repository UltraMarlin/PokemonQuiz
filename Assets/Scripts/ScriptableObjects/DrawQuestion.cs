using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawQuestion", menuName = "QuestionTypes/Draw", order = 5)]
public class DrawQuestion : IQuestion
{
    public string pokemonName;
    public Sprite sprite;

    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
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
