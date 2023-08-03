using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ShinyQuestion", menuName = "QuestionTypes/Shiny", order = 2)]
public class ShinyQuestion : IQuestion
{
    public Sprite solutionSprite;
    public List<Sprite> fakeSprites;
    public Sprite originalSprite;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        int pokemonNumber = 0;
        if (originalSprite != null)
        {
            string[] nameSplit = originalSprite.name.Split("_");
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
