using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamQuestion", menuName = "QuestionTypes/Team", order = 7)]
public class TeamQuestion : IQuestion
{
    public Sprite teamImage;
    public Sprite profileImage;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!QuizUtils.validateQuestionObjects) return;
        if (teamImage != null)
        {
            pokemonName = teamImage.name;
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
