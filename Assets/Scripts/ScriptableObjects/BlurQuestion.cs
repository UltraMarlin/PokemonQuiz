using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BlurQuestion", menuName = "QuestionTypes/Blur", order = 3)]
public class BlurQuestion : IQuestion
{
    public Sprite sprite;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        int pokemonNumber = 0;
        if (sprite != null)
        {
            string[] nameSplit = sprite.name.Split("_");
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
