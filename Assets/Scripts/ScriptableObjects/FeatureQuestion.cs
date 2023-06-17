using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureQuestion", menuName = "QuestionTypes/Feature", order = 1)]
public class FeatureQuestion : ScriptableObject, IQuestion
{
    public string pokemonName;
    public Sprite solutionSprite;
    public List<Sprite> featureSprites;

    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        if (solutionSprite != null)
        {
            pokemonName = solutionSprite.name.Split("_")[1];
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
        }
    }
}


