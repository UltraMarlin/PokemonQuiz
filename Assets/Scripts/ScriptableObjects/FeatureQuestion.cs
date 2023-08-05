using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "FeatureQuestion", menuName = "QuestionTypes/Feature", order = 1)]
public class FeatureQuestion : IQuestion
{
    public Sprite solutionSprite;
    public List<Sprite> featureSprites;
    public PokemonGen pokemonGen;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        int pokemonNumber = 0;
        if (solutionSprite != null)
        {
            string[] nameSplit = solutionSprite.name.Split("_");
            if (int.TryParse(nameSplit[0], out pokemonNumber))
            {
                int pokemonGenIndex = 0;
                while (pokemonGenIndex < QuizUtils.pokemonGenCutoffs.Length && pokemonNumber > QuizUtils.pokemonGenCutoffs[pokemonGenIndex])
                {
                    pokemonGenIndex++;
                }
                pokemonGen = (PokemonGen)pokemonGenIndex;
            }
            pokemonName = nameSplit[1];
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
        }
    }
#endif
}


