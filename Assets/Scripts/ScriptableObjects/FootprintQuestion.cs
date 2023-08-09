using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "FootprintQuestion", menuName = "QuestionTypes/Footprint", order = 6)]
public class FootprintQuestion : IQuestion
{
    public List<Sprite> sprites = new List<Sprite>();
    public Sprite solutionSprite;
    public Sprite footprintSprite;
    public Sprite portraitSprite;
    public List<int> footprintGroups;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        if (sprites.Count >= 3)
        {
            solutionSprite = sprites[0];
            footprintSprite = sprites[1];
            portraitSprite = sprites[2];

            footprintGroups = new List<int>();

            string[] nameSplit = footprintSprite.name.Split('_');
            pokemonName = nameSplit[1];
            if (int.TryParse(nameSplit[0], out int pokemonNumber))
            {
                int pokemonGenIndex = 0;
                while (pokemonGenIndex < QuizUtils.pokemonGenCutoffs.Length && pokemonNumber > QuizUtils.pokemonGenCutoffs[pokemonGenIndex])
                {
                    pokemonGenIndex++;
                }
                pokemonGen = (PokemonGen)pokemonGenIndex;
            }
            for (int i = 2; i < nameSplit.Length; i++)
            {
                int.TryParse(nameSplit[i], out int footprintGroup);
                footprintGroups.Add(footprintGroup);
            }
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
            name = pokemonName;
        }
    }
#endif
}
