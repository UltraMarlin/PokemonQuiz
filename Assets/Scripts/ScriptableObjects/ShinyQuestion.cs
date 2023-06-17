using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShinyQuestion", menuName = "QuestionTypes/Shiny", order = 2)]
public class ShinyQuestion : IQuestion
{
    public string pokemonName;
    public Sprite solutionSprite;
    public List<Sprite> fakeSprites;
    public Sprite originalSprite;
}
