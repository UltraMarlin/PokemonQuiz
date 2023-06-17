using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "DrawQuestion", menuName = "QuestionTypes/Draw", order = 5)]
public class DrawQuestion : IQuestion
{
    public string pokemonName;
    public string drawerName;
    public VideoClip videoClip;

    public void OnValidate()
    {
        //if (!QuizUtils.validateQuestionObjects) return;
        if (videoClip != null)
        {
            pokemonName = videoClip.name.Split('_')[0];
            drawerName = videoClip.name.Split('_')[1];
        }

        if (pokemonName.Length > 0)
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, pokemonName);
        }
    }
}
