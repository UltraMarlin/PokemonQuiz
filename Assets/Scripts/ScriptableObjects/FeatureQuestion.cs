using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureQuestion", menuName = "QuestionTypes/Feature", order = 1)]
public class FeatureQuestion : ScriptableObject, IQuestion
{
    public string pokemonName;
    public Sprite solutionSprite;
    public List<Sprite> featureSprites;
}
