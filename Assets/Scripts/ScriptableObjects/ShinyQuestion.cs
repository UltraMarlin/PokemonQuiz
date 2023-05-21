using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShinyQuestion", menuName = "QuestionTypes/Shiny", order = 2)]
public class ShinyQuestion : ScriptableObject
{
    public string pokemonName;
    public Sprite solutionSprite;
    public Sprite fakeSprite1;
    public Sprite fakeSprite2;
}
